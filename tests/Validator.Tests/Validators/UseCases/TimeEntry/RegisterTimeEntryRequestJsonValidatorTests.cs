using CommonTestUtilities.Requests;
using FluentAssertions;
using PeopleBank.Application.UseCases.TimeEntry;

namespace Validator.Tests.Validators.UseCases.TimeEntry;

public class RegisterTimeEntryRequestJsonValidatorTests
{
    private readonly RegisterTimeEntryUseCaseValidator _validator = new();

    [Fact] public void ShouldPass_WhenValid() => _validator.Validate(AddTimeEntryRequestJsonBuilder.Build()).IsValid.Should().BeTrue();

    [Fact] public void ShouldFail_WhenEmployeeIdEmpty()
    {
        var req = AddTimeEntryRequestJsonBuilder.Build();


        req.EmployeeId = Guid.Empty;

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenTimestampMissing()
    {
        var req = AddTimeEntryRequestJsonBuilder.Build();
        
        req.Timestamp = default;

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenInvalidType()
    {
        var req = AddTimeEntryRequestJsonBuilder.Build();
        
        req.Type = "Invalid";

        _validator.Validate(req).IsValid.Should().BeFalse();
    }
}