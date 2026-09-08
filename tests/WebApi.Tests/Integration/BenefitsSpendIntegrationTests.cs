using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Enums;
using PeopleBank.WebApi.Tests.Factories;

namespace PeopleBank.WebApi.Tests.Integration;

public class BenefitsSpendIntegrationTests : IntegrationTestBase
{
    public BenefitsSpendIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task SpendBenefit_ShouldReturnOk_WhenValid()
    {
        var company = await SeedCompanyAsync();

        var department = await SeedDepartmentAsync(company.Id);

        var position = await SeedPositionAsync(company.Id);

        var employee = await SeedEmployeeAsync(company.Id, department.Id, position.Id);

        var account = await SeedAccountAsync(employee.Id, 1000);

        var benefitDef = await SeedBenefitDefinitionAsync(company.Id, BenefitCategory.Meal, 500);
        var wallet = await SeedBenefitWalletAsync(account.Id, benefitDef.Id, 300);

        var request = new SpendBenefitRequestJson
        {
            AccountId = account.Id,
            BenefitCategory = BenefitCategory.Meal,
            Amount = 100,
            EstablishmentCategory = EstablishmentCategory.Restaurant
        };

        var response = await Client.PostAsJsonAsync("/api/benefits/spend", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<SpendBenefitResponseJson>();

        body.Should().NotBeNull();

        body!.NewBalance.Should().Be(200);

        body.BenefitCategory.Should().Be(BenefitCategory.Meal.ToString());
    }
}