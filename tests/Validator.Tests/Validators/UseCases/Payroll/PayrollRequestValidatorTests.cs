using CommonTestUtilities.Requests;
using FluentAssertions;
using PeopleBank.Application.UseCases.Payroll;
using PeopleBank.Communication.Requests;

namespace Validator.Tests.Validators.UseCases.Payroll;

public class PayrollRequestValidatorTests
{
    private readonly ProcessPayrollUseCaseValidator _validator = new();

    [Fact]
    public void ShouldPass_WhenValidRequest()
    {
        var request = AddProcessPayrollRequestJsonBuilder.Build();

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldFail_WhenCompanyIdEmpty()
    {
        var request = AddProcessPayrollRequestJsonBuilder.Build();
        request.CompanyId = Guid.Empty;

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ProcessPayrollRequestJson.CompanyId));
    }

    [Fact]
    public void ShouldFail_WhenMonthOutOfRange()
    {
        var request = AddProcessPayrollRequestJsonBuilder.Build();
        request.Month = 13;

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ProcessPayrollRequestJson.Month));
    }

    [Fact]
    public void ShouldFail_WhenYearInvalid()
    {
        var request = AddProcessPayrollRequestJsonBuilder.Build();
        request.Year = 1800;

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ProcessPayrollRequestJson.Year));
    }
}