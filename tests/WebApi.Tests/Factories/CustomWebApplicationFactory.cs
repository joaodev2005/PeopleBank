using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PeopleBank.Domain.Interfaces.Services;
using StackExchange.Redis;
using Testcontainers.MsSql;

namespace PeopleBank.WebApi.Tests.Factories;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer;

    public CustomWebApplicationFactory()
    {
        _sqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-CU10-ubuntu-22.04")
            .WithPassword("DevPass123!")
            .WithDatabase("PeopleBankTest")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Tests")
            .ConfigureAppConfiguration((_, configuration) =>
            {
                var parameters = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DbConnection"] = _sqlContainer.GetConnectionString(),
                    ["ConnectionStrings:Redis"] = "localhost:6379",
                    ["SkipMigrations"] = "true"         
                };
                configuration.AddInMemoryCollection(parameters);
            })
            .ConfigureServices(services =>
            {
                var redisDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IConnectionMultiplexer));

                if (redisDescriptor != null) services.Remove(redisDescriptor);

                var mockRedis = new Mock<IConnectionMultiplexer>();

                var mockDb = new Mock<IDatabase>();

                mockDb.Setup(d => d.LockTakeAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), It.IsAny<CommandFlags>()))
      .ReturnsAsync(true);

                mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                         .Returns(mockDb.Object);

                services.AddSingleton(mockRedis.Object);

                var kafkaDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IKafkaProducer));

                if (kafkaDescriptor != null) services.Remove(kafkaDescriptor);

                var mockKafka = new Mock<IKafkaProducer>();

                services.AddSingleton(mockKafka.Object);
            });
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging();

        serviceCollection.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(_sqlContainer.GetConnectionString())
                .ScanIn(Assembly.Load("PeopleBank.Infrastructure")).For.Migrations());

        using var provider = serviceCollection.BuildServiceProvider();

        using var scope = provider.CreateScope();

        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        runner.MigrateUp();
    }

    Task IAsyncLifetime.DisposeAsync() => _sqlContainer.DisposeAsync().AsTask();
}