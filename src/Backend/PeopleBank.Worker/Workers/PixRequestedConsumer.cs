using Confluent.Kafka;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PeopleBank.Domain.Events;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;

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
                try
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (result == null) continue;

                    _logger.LogInformation("Processando Pix no offset {Offset}", result.Offset);

                    var pixEvent = JsonSerializer.Deserialize<PixRequestedEvent>(result.Message.Value);
                    if (pixEvent == null)
                    {
                        _logger.LogWarning("Mensagem inválida no offset {Offset}", result.Offset);
                        continue;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var accountRepo = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
                    var transactionRepo = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var sourceAccount = await accountRepo.GetByIdAsync(pixEvent.SourceAccountId);
                    var targetAccount = await accountRepo.GetByIdAsync(pixEvent.TargetAccountId);

                    if (sourceAccount == null || targetAccount == null)
                    {
                        _logger.LogWarning("Conta não encontrada para a transação {TransactionId}", pixEvent.TransactionId);
                        continue;
                    }

                    sourceAccount.InternalDebit(pixEvent.Amount);
                    targetAccount.Credit(pixEvent.Amount);

                    var transaction = await transactionRepo.GetByIdempotencyKeyAsync(pixEvent.IdempotencyKey);
                    if (transaction != null)
                    {
                        transaction.MarkAsCompleted();
                    }

                    await unitOfWork.SaveChangesAsync();

                    consumer.Commit(result);
                    _logger.LogInformation("Pix efetivado: {TransactionId}", pixEvent.TransactionId);
                }
                catch (OperationCanceledException) { break; }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar Pix");
                }
            }
        }, stoppingToken);
    }
}