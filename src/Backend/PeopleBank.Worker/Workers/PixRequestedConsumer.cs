using System.Text.Json;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.Events;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Worker.Workers;

public class PixRequestedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PixRequestedConsumer> _logger;

    public PixRequestedConsumer(
        IServiceScopeFactory scopeFactory,
        ILogger<PixRequestedConsumer> logger)
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
            consumer.Subscribe("pix-requested");
            _logger.LogInformation("Inscrito no tópico pix-requested");

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result = null;
                try
                {
                    result = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (result == null) continue;

                    _logger.LogInformation("Processando Pix no offset {Offset}", result.Offset);

                    var pixEvent = JsonSerializer.Deserialize<PixRequestedEvent>(result.Message.Value);
                    if (pixEvent == null)
                    {
                        _logger.LogWarning("Mensagem inválida no offset {Offset}", result.Offset);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(pixEvent.IdempotencyKey))
                    {
                        _logger.LogWarning("IdempotencyKey ausente no evento Pix no offset {Offset}", result.Offset);
                        continue;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<PeopleBankDbContext>();

                    using var transaction = await dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var existingTransaction = await dbContext.Transactions
                            .FirstOrDefaultAsync(t => t.IdempotencyKey == pixEvent.IdempotencyKey && t.Status == TransactionStatus.Completed);

                        if (existingTransaction != null)
                        {
                            _logger.LogInformation("Transação já processada (idempotência): {IdempotencyKey}", pixEvent.IdempotencyKey);
                            await transaction.CommitAsync();
                            consumer.Commit(result);
                            continue;
                        }

                        var sourceAccount = await dbContext.Accounts
                            .FirstOrDefaultAsync(a => a.Id == pixEvent.SourceAccountId);

                        if (sourceAccount == null)
                        {
                            throw new InvalidOperationException("ACCOUNT_NOT_FOUND: Conta de origem não encontrada");
                        }

                        if (!sourceAccount.Active)
                        {
                            throw new InvalidOperationException("ACCOUNT_INACTIVE: Conta de origem inativa");
                        }

                        var targetAccount = await dbContext.Accounts
                            .FirstOrDefaultAsync(a => a.Id == pixEvent.TargetAccountId);

                        if (targetAccount == null)
                        {
                            throw new InvalidOperationException("ACCOUNT_NOT_FOUND: Conta de destino não encontrada");
                        }

                        if (!targetAccount.Active)
                        {
                            throw new InvalidOperationException("ACCOUNT_INACTIVE: Conta de destino inativa");
                        }

                        sourceAccount.InternalDebit(pixEvent.Amount);
                        targetAccount.Credit(pixEvent.Amount);

                        var newTransaction = new Transaction(
                            pixEvent.SourceAccountId,
                            pixEvent.TargetAccountId,
                            pixEvent.Amount,
                            pixEvent.IdempotencyKey,
                            pixEvent.Description);

                        newTransaction.MarkAsCompleted();
                        dbContext.Transactions.Add(newTransaction);

                        await dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();

                        consumer.Commit(result);
                        _logger.LogInformation("Pix efetivado: {TransactionId}, IdempotencyKey: {IdempotencyKey}", 
                            newTransaction.Id, pixEvent.IdempotencyKey);
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Erro de concorrência ao processar Pix");
                    if (result != null) consumer.Commit(result);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "JSON inválido no tópico pix-requested. Mensagem ignorada.");
                    if (result != null) consumer.Commit(result);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("ACCOUNT_NOT_FOUND") || ex.Message.Contains("ACCOUNT_INACTIVE") || ex.Message.Contains("Insufficient"))
                {
                    _logger.LogWarning(ex, "Erro de validação ao processar Pix: {Message}", ex.Message);
                    if (result != null) consumer.Commit(result);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar Pix");
                }
            }
        }, stoppingToken);
    }
}