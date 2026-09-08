using CommonTestUtilities.Requests;
using FluentAssertions;
using PeopleBank.Application.UseCases.Benefits.SpendBenefits;
using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Enums;

namespace Validator.Tests.Validators.UseCases.Benefits.SpendBenefit;

public class SpendBenefitRequestValidatorTests
{
    private readonly SpendBenefitUseCaseValidator _validator = new();

    [Fact]
    public void ShouldPass_WhenValidRequest()
    {
        var request = AddSpendBenefitRequestJsonBuilder.Build();

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldFail_WhenAccountIdEmpty()
    {
        var request = AddSpendBenefitRequestJsonBuilder.Build();
        request.AccountId = Guid.Empty;

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SpendBenefitRequestJson.AccountId));
    }

    [Fact]
    public void ShouldFail_WhenAmountZeroOrNegative()
    {
        var request = AddSpendBenefitRequestJsonBuilder.Build();
        request.Amount = 0;

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SpendBenefitRequestJson.Amount));
    }

    [Fact]
    public void ShouldFail_WhenInvalidBenefitCategory()
    {
        var request = AddSpendBenefitRequestJsonBuilder.Build();
        request.BenefitCategory = (BenefitCategory)999;

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SpendBenefitRequestJson.BenefitCategory));
    }

    [Fact]
    public void ShouldFail_WhenInvalidEstablishmentCategory()
    {
        var request = AddSpendBenefitRequestJsonBuilder.Build();
        request.EstablishmentCategory = (EstablishmentCategory)999;

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SpendBenefitRequestJson.EstablishmentCategory));
    }
}