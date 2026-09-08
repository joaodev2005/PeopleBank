using CommonTestUtilities.Requests;
using FluentAssertions;
using PeopleBank.Application.UseCases.Pix;

namespace Validator.Tests.Validators.UseCases.Pix;

public class RequestPixJsonValidatorTests
{
    private readonly RequestPixUseCaseValidator _validator = new();

    [Fact] public void ShouldPass_WhenValid() => _validator.Validate(AddPixJsonBuilder.Build()).IsValid.Should().BeTrue();

    [Fact] public void ShouldFail_WhenSourceAccountIdEmpty()
    {
        var req = AddPixJsonBuilder.Build(); 
        req.SourceAccountId = Guid.Empty;

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenTargetPixKeyEmpty()
    {
        var req = AddPixJsonBuilder.Build();   
        req.TargetPixKey = "";

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenAmountZero()
    {
        var req = AddPixJsonBuilder.Build(); 
        req.Amount = 0;

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenIdempotencyKeyEmpty()
    {
        var req = AddPixJsonBuilder.Build(); 
        req.IdempotencyKey = "";

        _validator.Validate(req).IsValid.Should().BeFalse();
    }
}