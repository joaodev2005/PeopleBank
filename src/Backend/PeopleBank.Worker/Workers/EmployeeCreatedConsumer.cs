using Confluent.Kafka;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PeopleBank.Application.UseCases.Account;
using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Events;

namespace PeopleBank.Worker.Workers;

public class EmployeeCreatedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmployeeCreatedConsumer> _logger;

    public EmployeeCreatedConsumer(
        IServiceScopeFactory scopeFactory,
        ILogger<EmployeeCreatedConsumer> logger)
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
            consumer.Subscribe("employee-created");
            _logger.LogInformation("Inscrito no tópico employee-created");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker aguardando mensagens...");

                try
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result?.Message == null) continue;

                    _logger.LogInformation("Mensagem recebida no offset {Offset}", result.Offset);

                    var employeeEvent = JsonSerializer.Deserialize<EmployeeCreatedEvent>(result.Message.Value);
                    if (employeeEvent == null)
                    {
                        _logger.LogWarning("Mensagem inválida no offset {Offset}", result.Offset);
                        continue;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var useCase = scope.ServiceProvider.GetRequiredService<IOpenAccountUseCase>();

                    var request = new OpenAccountRequestJson
                    {
                        EmployeeId = employeeEvent.EmployeeId,
                        PixKey = employeeEvent.PixKey,
                        PixKeyType = employeeEvent.PixKeyType
                    };

                    await useCase.Execute(request);
                    consumer.Commit(result);

                    _logger.LogInformation("Conta aberta automaticamente para o funcionário {EmployeeId}", employeeEvent.EmployeeId);
                }
                catch (OperationCanceledException) { break; }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar evento EmployeeCreated");
                }
            }
        }, stoppingToken);
    }
}
