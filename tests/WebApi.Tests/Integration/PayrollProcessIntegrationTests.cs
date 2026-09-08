using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PeopleBank.Communication.Requests;
using PeopleBank.WebApi.Tests.Factories;

namespace PeopleBank.WebApi.Tests.Integration;

public class PayrollProcessIntegrationTests : IntegrationTestBase
{
    public PayrollProcessIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task ProcessPayroll_ShouldReturnAccepted_WhenValid()
    {
        var company = await SeedCompanyAsync();

        var request = new ProcessPayrollRequestJson
        {
            CompanyId = company.Id,
            Month = 8,
            Year = 2026
        };

        var response = await Client.PostAsJsonAsync("/api/payroll/process", request);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}