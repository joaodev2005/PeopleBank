using CommonTestUtilities.Requests;
using FluentAssertions;
using PeopleBank.Application.UseCases.Employee;
using PeopleBank.Domain.Enums;

namespace Validator.Tests.Validators.UseCases.Employee;

public class AddEmployeeRequestJsonValidatorTests
{
    private readonly AddEmployeeUseCaseValidator _validator = new();

    [Fact]
    public void ShouldPass_WhenValid()
    {
        var req = AddEmployeeRequestJsonBuilder.Build();

        _validator.Validate(req).IsValid.Should().BeTrue();
    }

    [Fact] public void ShouldFail_WhenNameEmpty()
    {
        var req = AddEmployeeRequestJsonBuilder.Build();

        req.Name = "";

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenCpfInvalidLength()
    {
        var req = AddEmployeeRequestJsonBuilder.Build();
        
        req.Cpf = "123";

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenEmailInvalid()
    {
        var req = AddEmployeeRequestJsonBuilder.Build();
        
        req.Email = "bad";

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenSalaryZero()
    {
        var req = AddEmployeeRequestJsonBuilder.Build();

        req.Salary = 0;

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenInvalidPixKeyType()
    {
        var req = AddEmployeeRequestJsonBuilder.Build();
        
        req.PixKeyType = (PixKeyType)99;

        _validator.Validate(req).IsValid.Should().BeFalse();
    }

    [Fact] public void ShouldFail_WhenMissingIds()
    {
        var req = AddEmployeeRequestJsonBuilder.Build();
        
        req.CompanyId = Guid.Empty; req.DepartmentId = Guid.Empty; req.PositionId = Guid.Empty;

        _validator.Validate(req).IsValid.Should().BeFalse();
    }
}