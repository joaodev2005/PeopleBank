using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Infrastructure.Data;
using PeopleBank.Infrastructure.Repositories;
using PeopleBank.Infrastructure.Services;
using StackExchange.Redis;

namespace PeopleBank.Infrastructure;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PeopleBankDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DbConnection")!;
            options.UseSqlServer(connectionString);
        });

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(configuration.GetConnectionString("DbConnection"))
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddRepositories();

        services.AddInfrastructureServices(configuration);

        return services;
    }

private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();
            services.AddScoped<IPayrollRepository, PayrollRepository>();
            services.AddScoped<IBenefitDefinitionRepository, BenefitDefinitionRepository>();
            services.AddScoped<IBenefitWalletRepository, BenefitWalletRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
        }

    private static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConn = configuration.GetConnectionString("Redis")!;
            var options = ConfigurationOptions.Parse(redisConn);
            options.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(options);
        });
        services.AddScoped<IRedisService, RedisService>();

        services.AddSingleton<IKafkaProducer>(sp =>
        {
            var bootstrapServers = configuration["Kafka:BootstrapServers"]!;
            return new KafkaProducer(bootstrapServers);
        });
    }
}