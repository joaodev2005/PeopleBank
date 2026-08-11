using System.Text.Json;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.Events;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Worker.Workers;

public class PayrollRequestedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PayrollRequestedConsumer> _logger;

    public PayrollRequestedConsumer(
        IServiceScopeFactory scopeFactory,
        ILogger<PayrollRequestedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var consumerScope = _scopeFactory.CreateScope();
            var consumer = consumerScope.ServiceProvider.GetRequiredService<IConsumer<string, string>>();
            consumer.Subscribe("payroll-requested");
            _logger.LogInformation("Inscrito no tópico payroll-requested");

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result = null;
                try
                {
                    result = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (result == null) continue;

                    _logger.LogInformation("Processando folha no offset {Offset}", result.Offset);

                    var payrollEvent = JsonSerializer.Deserialize<PayrollRequestedEvent>(result.Message.Value);
                    if (payrollEvent == null) continue;

                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<PeopleBankDbContext>();

                    var payroll = await dbContext.Payrolls
                        .FirstOrDefaultAsync(p => p.Id == payrollEvent.PayrollId);

                    if (payroll == null || payroll.Status == PayrollStatus.Processed)
                    {
                        _logger.LogWarning("Folha já processada ou não encontrada: {PayrollId}", payrollEvent.PayrollId);
                        continue;
                    }

                    var employees = await dbContext.Employees
                        .Where(e => e.CompanyId == payrollEvent.CompanyId && e.Active)
                        .ToListAsync();

                    decimal totalPaid = 0;
                    var accountIds = new List<Guid>();
                    var payslipIds = new List<Guid>();
                    var transactionIds = new List<Guid>();

                    foreach (var employee in employees)
                    {
                        var account = await dbContext.Accounts
                            .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id);

                        if (account == null)
                        {
                            _logger.LogWarning("Funcionário {EmployeeId} sem conta, ignorado", employee.Id);
                            continue;
                        }
                        accountIds.Add(account.Id);

                        var timeEntries = await dbContext.TimeEntries
                            .Where(t => t.EmployeeId == employee.Id
                                        && t.Timestamp.Month == payrollEvent.Month
                                        && t.Timestamp.Year == payrollEvent.Year)
                            .OrderBy(t => t.Timestamp)
                            .ToListAsync();

                        int overtimeHours = CalculateOvertimeHours(timeEntries);
                        decimal overtimeAmount = overtimeHours * (employee.Salary / 160m * 1.5m);

                        decimal baseSalary = employee.Salary;
                        decimal discounts = baseSalary * 0.10m;
                        decimal netSalary = baseSalary + overtimeAmount - discounts;

                        var payslip = new Payslip(
                            payroll.Id,
                            employee.Id,
                            baseSalary,
                            overtimeHours,
                            overtimeAmount,
                            discounts
                        );
                        dbContext.Payslips.Add(payslip);
                        payroll.AddPayslip(payslip);
                        payslipIds.Add(payslip.Id);

                        account.Credit(netSalary);
                        var creditTransaction = new Transaction(
                            account.Id,
                            null,
                            netSalary,
                            $"payroll-{payroll.Id}-{employee.Id}",
                            "Salary"
                        );
                        dbContext.Transactions.Add(creditTransaction);
                        creditTransaction.MarkAsCompleted();
                        account.AddTransaction(creditTransaction);
                        transactionIds.Add(creditTransaction.Id);

                        using (var benefitTransaction = await dbContext.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                var activeBenefits = await dbContext.BenefitDefinitions
                                    .Where(bd => bd.CompanyId == payrollEvent.CompanyId && bd.Active)
                                    .ToListAsync();

                                foreach (var benefitDef in activeBenefits)
                                {
                                    var idempotencyKey = $"ben-{payrollEvent.PayrollId:N}-{account.Id:N}-{benefitDef.Id:N}"
                     .Substring(0, 100);
                                    var existingTx = await dbContext.Transactions
                                        .FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey);
                                    if (existingTx != null) continue;

                                    var existingWallet = account.BenefitWallets
                                        .FirstOrDefault(w => w.BenefitDefinition.Category == benefitDef.Category
                                                             && w.ExpirationDate > DateTime.UtcNow);

                                    if (existingWallet == null)
                                    {
                                        var expirationDate = new DateTime(payrollEvent.Year, payrollEvent.Month, 1)
                                            .AddMonths(2).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                                        existingWallet = new BenefitWallet(account.Id, benefitDef.Id,
                                            benefitDef.MonthlyAmount, expirationDate);
                                        account.AddBenefitWallet(existingWallet);
                                        dbContext.BenefitWallets.Add(existingWallet);
                                    }
                                    else
                                    {
                                        existingWallet.Credit(benefitDef.MonthlyAmount);
                                    }

                                    var benefitTx = new Transaction(
                                        account.Id,
                                        null,
                                        benefitDef.MonthlyAmount,
                                        idempotencyKey,
                                        $"Benefit: {benefitDef.Name}");
                                    dbContext.Transactions.Add(benefitTx);
                                    benefitTx.MarkAsCompleted();
                                    account.AddTransaction(benefitTx);
                                    transactionIds.Add(benefitTx.Id);
                                }

                                await benefitTransaction.CommitAsync();
                            }
                            catch
                            {
                                await benefitTransaction.RollbackAsync();
                                throw;
                            }
                        }

                        totalPaid += netSalary;
                    }

                    payroll.MarkAsProcessed();
                    _logger.LogInformation("Salvando folha {PayrollId}. Contas: {@AccountIds}. Holerites: {@PayslipIds}. Transações: {@TransactionIds}", payroll.Id, accountIds, payslipIds, transactionIds);
                    await dbContext.SaveChangesAsync();

                    consumer.Commit(result);
                    _logger.LogInformation("Folha processada: {PayrollId}, total pago: {TotalPaid}", payroll.Id, totalPaid);
                }
                catch (OperationCanceledException) { break; }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entries = ex.Entries.Select(entry => new { Entity = entry.Entity.GetType().Name, State = entry.State.ToString() }).ToList();
                    _logger.LogError(ex, "Erro de concorrência ao processar folha. Entradas: {@Entries}", entries);
                    if (result != null) consumer.Commit(result);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "JSON inválido no tópico. Mensagem ignorada.");
                    if (result != null) consumer.Commit(result);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar folha");
                    if (result != null) consumer.Commit(result);
                }
            }
        }, stoppingToken);
    }

    private static int CalculateOvertimeHours(IEnumerable<TimeEntry> entries)
    {
        var grouped = entries.OrderBy(e => e.Timestamp)
                            .GroupBy(e => e.Timestamp.Date);
        int overtime = 0;
        foreach (var day in grouped)
        {
            var entriesDay = day.OrderBy(e => e.Timestamp).ToList();
            TimeSpan worked = TimeSpan.Zero;
            for (int i = 0; i < entriesDay.Count - 1; i += 2)
            {
                if (entriesDay[i].Type == TimeEntryType.ClockIn &&
                    entriesDay[i + 1].Type == TimeEntryType.ClockOut)
                {
                    worked += entriesDay[i + 1].Timestamp - entriesDay[i].Timestamp;
                }
            }
            if (worked.TotalHours > 8)
                overtime += (int)(worked.TotalHours - 8);
        }
        return overtime;
    }
}
