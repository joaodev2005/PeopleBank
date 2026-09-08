using CommonTestUtilities.Requests;
using FluentAssertions;
using PeopleBank.Application.UseCases.Account.OpenAccount;

namespace Validator.Tests.Validators.UseCases.Account;

public class OpenAccountRequestJsonValidatorTests
{
    private readonly OpenAccountUseCaseValidator _validator = new();

    [Fact] public void ShouldPass_WhenValid() => _validator.Validate(AddAccountRequestJsonBuilder.Build()).IsValid.Should().BeTrue();

    [Fact] public void ShouldFail_WhenEmployeeIdEmpty()
    {
        var req = AddAccountRequestJsonBuilder.Build();
        
        req.EmployeeId = Guid.Empty;

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenPixKeyEmpty()
    {
        var req = AddAccountRequestJsonBuilder.Build(); 
        
        req.PixKey = "";

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenPixKeyTypeInvalid()
    {
        var req = AddAccountRequestJsonBuilder.Build(); 
        
        req.PixKeyType = "Invalid";

        _validator.Validate(req).IsValid.Should().BeFalse();
    }
}