//using System.Net;
//using System.Net.Http.Json;
//using FluentAssertions;
//using Microsoft.Extensions.DependencyInjection;
//using PeopleBank.CommonTestUtilities.Builders;
//using PeopleBank.Communication.Requests;
//using PeopleBank.Domain.Enums;
//using PeopleBank.Infrastructure.Data;
//using PeopleBank.WebApi.Tests.Factories;

//namespace PeopleBank.WebApi.Tests.Integration;

//public class PixRequestIntegrationTests : IClassFixture<CustomWebApplicationFactory>
//{
//    private readonly HttpClient _client;
//    private readonly CustomWebApplicationFactory _factory;

//    public PixRequestIntegrationTests(CustomWebApplicationFactory factory)
//    {
//        _factory = factory;
//        _client = factory.CreateClient();
//    }

//    [Fact]
//    public async Task RequestPix_ShouldReturnAccepted_WhenValid()
//    {
//        using var scope = _factory.Services.CreateScope();

//        var db = scope.ServiceProvider.GetRequiredService<PeopleBankDbContext>();

//        var company = new CompanyBuilder().Build();

//        db.Companies.Add(company);

//        await db.SaveChangesAsync();

//        var department = new DepartmentBuilder().WithCompanyId(company.Id).Build();

//        var position = new PositionBuilder().WithCompanyId(company.Id).Build();

//        db.Departments.Add(department);

//        db.Positions.Add(position);

//        await db.SaveChangesAsync();

//        var employee = new EmployeeBuilder()
//            .WithCompanyId(company.Id)
//            .WithDepartmentId(department.Id)
//            .WithPositionId(position.Id)
//            .WithCpf("52998224725")   
//            .Build();

//        db.Employees.Add(employee);

//        await db.SaveChangesAsync();

//        var source = new AccountBuilder()
//            .WithEmployeeId(employee.Id)
//            .WithBalance(1000)
//            .Build();

//        var targetEmployee = new EmployeeBuilder()
//            .WithCompanyId(company.Id)
//            .WithDepartmentId(department.Id)
//            .WithPositionId(position.Id)
//            .WithCpf("52035968836")  
//            .Build();

//        db.Employees.Add(targetEmployee);

//        await db.SaveChangesAsync();

//        var target = new AccountBuilder()
//            .WithEmployeeId(targetEmployee.Id)
//            .WithPixKey("target-pix", PixKeyType.CPF)
//            .Build();

//        db.Accounts.AddRange(source, target);

//        await db.SaveChangesAsync();

//        var request = new RequestPixJson
//        {
//            SourceAccountId = source.Id,
//            TargetPixKey = "target-pix",
//            Amount = 100,
//            IdempotencyKey = "idem-integration-1"
//        };

//        _client.DefaultRequestHeaders.TryAddWithoutValidation("Idempotency-Key", "idem-integration-1");

//        var response = await _client.PostAsJsonAsync("/api/pix", request);

//        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
//    }
//}

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Enums;
using PeopleBank.WebApi.Tests.Factories;

namespace PeopleBank.WebApi.Tests.Integration;

public class PixRequestIntegrationTests : IntegrationTestBase
{
    public PixRequestIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task RequestPix_ShouldReturnAccepted_WhenValid()
    {
        var company = await SeedCompanyAsync();

        var department = await SeedDepartmentAsync(company.Id);

        var position = await SeedPositionAsync(company.Id);

        var employee1 = await SeedEmployeeAsync(
            company.Id, department.Id, position.Id, "52998224725");

        var sourceAccount = await SeedAccountAsync(
            employee1.Id, balance: 1000);

        var employee2 = await SeedEmployeeAsync(
            company.Id, department.Id, position.Id, "52035968836");

        var targetAccount = await SeedAccountAsync(
            employee2.Id, pixKey: "target-pix", pixType: PixKeyType.CPF);

        var request = new RequestPixJson
        {
            SourceAccountId = sourceAccount.Id,
            TargetPixKey = "target-pix",
            Amount = 100,
            IdempotencyKey = "idem-integration-1"
        };

        Client.DefaultRequestHeaders.TryAddWithoutValidation(
            "Idempotency-Key", "idem-integration-1");

        var response = await Client.PostAsJsonAsync("/api/pix", request);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}