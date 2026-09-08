using CommonTestUtilities.Requests;
using FluentAssertions;
using PeopleBank.Application.UseCases.Benefits.DefineBenefits;
using PeopleBank.Domain.Enums;

namespace Validator.Tests.Validators.UseCases.Benefits.DefineBenefit;

public class DefineBenefitRequestJsonValidatorTests
{
    private readonly DefineBenefitsUseCaseValidator _validator = new();

    [Fact] public void ShouldPass_WhenValid() => _validator.Validate(AddBenefitsRequestJsonBuilder.Build()).IsValid.Should().BeTrue();

    [Fact] public void ShouldFail_WhenCompanyIdEmpty()
    {
        var req = AddBenefitsRequestJsonBuilder.Build(); 
        req.CompanyId = Guid.Empty;
        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenNameEmpty()
    {
        var req = AddBenefitsRequestJsonBuilder.Build(); 
        req.Name = "";
        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenInvalidCategory()
    {
        var req = AddBenefitsRequestJsonBuilder.Build(); 
        req.Category = (BenefitCategory)5;
        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenMonthlyAmountZero()
    {
        var req = AddBenefitsRequestJsonBuilder.Build(); 
        req.MonthlyAmount = 0;
        _validator.Validate(req).IsValid.Should().BeFalse();
    }
}