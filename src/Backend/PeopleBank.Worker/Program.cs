using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeopleBank.Application;
using PeopleBank.Infrastructure;
using PeopleBank.Worker.Workers;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddApplication();
        services.AddInfrastructure(context.Configuration);

        services.AddTransient<IConsumer<string, string>>(sp =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = context.Configuration["Kafka:BootstrapServers"],
                GroupId = "peoplebank-worker-v4",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
            return new ConsumerBuilder<string, string>(config).Build();
        });

        services.AddHostedService<EmployeeCreatedConsumer>();
        services.AddHostedService<PixRequestedConsumer>();
        services.AddHostedService<PayrollRequestedConsumer>();
    })
    .Build();

await host.RunAsync();
