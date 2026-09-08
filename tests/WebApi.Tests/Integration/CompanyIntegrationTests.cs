using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PeopleBank.CommonTestUtilities.Builders;
using PeopleBank.Communication.Requests;
using PeopleBank.WebApi.Tests.Factories;

namespace PeopleBank.WebApi.Tests.Integration;

public class CompanyIntegrationTests : IntegrationTestBase
{
    public CompanyIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateCompany_ShouldReturnCreated()
    {
        var companyBuilder = new CompanyBuilder().Build();
        var request = new RegisterCompanyRequestJson
        {
            Name = companyBuilder.Name,
            Cnpj = companyBuilder.Cnpj.Value
        };

        var response = await Client.PostAsJsonAsync("/api/companies", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateCompany_DuplicateCnpj_ShouldReturnBadRequest()
    {
        var request = new RegisterCompanyRequestJson
        {
            Name = "Empresa Teste",
            Cnpj = "11222333000181"
        };

        await Client.PostAsJsonAsync("/api/companies", request);

        var response = await Client.PostAsJsonAsync("/api/companies", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
