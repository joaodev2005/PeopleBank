using CommonTestUtilities.Requests;
using FluentAssertions;
using PeopleBank.Application.UseCases.Company;

namespace Validator.Tests.Validators.UseCases.Company;

public class RegisterCompanyRequestJsonValidatorTests
{
    private readonly RegisterCompanyUseCaseValidator _validator = new();

    [Fact]
    public void ShouldPass_WhenValid()
    {
        var req = AddCompanyRequestJsonBuilder.Build();

        _validator.Validate(req).IsValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldFail_WhenNameEmpty()
    {
        var req = AddCompanyRequestJsonBuilder.Build();
        req.Name = "";

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ShouldFail_WhenCnpjInvalidLength()
    {
        var req = AddCompanyRequestJsonBuilder.Build();
        req.Cnpj = "123";

        _validator.Validate(req).IsValid.Should().BeFalse();
    }
}