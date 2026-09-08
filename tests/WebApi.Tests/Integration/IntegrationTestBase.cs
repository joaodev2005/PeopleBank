using Microsoft.Extensions.DependencyInjection;
using PeopleBank.CommonTestUtilities.Builders;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;
using PeopleBank.Infrastructure.Data;
using PeopleBank.WebApi.Tests.Factories;

namespace PeopleBank.WebApi.Tests.Integration;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task<Company> SeedCompanyAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeopleBankDbContext>();

        var company = new CompanyBuilder().Build();
        db.Companies.Add(company);
        await db.SaveChangesAsync();
        return company;
    }

    protected async Task<Department> SeedDepartmentAsync(Guid companyId)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeopleBankDbContext>();

        var department = new DepartmentBuilder().WithCompanyId(companyId).Build();
        db.Departments.Add(department);
        await db.SaveChangesAsync();
        return department;
    }

    protected async Task<Position> SeedPositionAsync(Guid companyId)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeopleBankDbContext>();

        var position = new PositionBuilder().WithCompanyId(companyId).Build();
        db.Positions.Add(position);
        await db.SaveChangesAsync();
        return position;
    }

    protected async Task<Employee> SeedEmployeeAsync(
        Guid companyId,
        Guid departmentId,
        Guid positionId,
        string? cpf = null)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeopleBankDbContext>();

        var builder = new EmployeeBuilder()
            .WithCompanyId(companyId)
            .WithDepartmentId(departmentId)
            .WithPositionId(positionId);

        if (!string.IsNullOrEmpty(cpf))
            builder.WithCpf(cpf);

        var employee = builder.Build();
        db.Employees.Add(employee);
        await db.SaveChangesAsync();
        return employee;
    }

    protected async Task<Account> SeedAccountAsync(
        Guid employeeId,
        decimal balance = 0,
        string? pixKey = null,
        PixKeyType pixType = PixKeyType.Email)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeopleBankDbContext>();

        var builder = new AccountBuilder()
            .WithEmployeeId(employeeId)
            .WithBalance(balance);

        if (!string.IsNullOrEmpty(pixKey))
            builder.WithPixKey(pixKey, pixType);

        var account = builder.Build();
        db.Accounts.Add(account);
        await db.SaveChangesAsync();
        return account;
    }

    protected async Task<BenefitDefinition> SeedBenefitDefinitionAsync(
        Guid companyId,
        BenefitCategory category,
        decimal monthlyAmount)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeopleBankDbContext>();

        var benefitDef = new BenefitDefinitionBuilder()
            .WithCompanyId(companyId)
            .WithCategory(category)
            .WithMonthlyAmount(monthlyAmount)
            .Build();
        db.BenefitDefinitions.Add(benefitDef);
        await db.SaveChangesAsync();
        return benefitDef;
    }

    protected async Task<BenefitWallet> SeedBenefitWalletAsync(
        Guid accountId,
        Guid benefitDefinitionId,
        decimal balance)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeopleBankDbContext>();

        var wallet = new BenefitWalletBuilder()
            .WithAccountId(accountId)
            .WithBenefitDefinitionId(benefitDefinitionId)
            .WithBalance(balance)
            .Build();
        db.BenefitWallets.Add(wallet);
        await db.SaveChangesAsync();
        return wallet;
    }

    protected async Task<Account> SeedFullAccountAsync(
        decimal balance = 0,
        string? cpf = null,
        string? pixKey = null,
        PixKeyType pixType = PixKeyType.Email)
    {
        var company = await SeedCompanyAsync();
        var department = await SeedDepartmentAsync(company.Id);
        var position = await SeedPositionAsync(company.Id);
        var employee = await SeedEmployeeAsync(company.Id, department.Id, position.Id, cpf);
        var account = await SeedAccountAsync(employee.Id, balance, pixKey, pixType);
        return account;
    }
}