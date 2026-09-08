using FluentValidation;
using PeopleBank.Communication.Requests;

namespace PeopleBank.Application.UseCases.Employee;

public class AddEmployeeUseCaseValidator : AbstractValidator<AddEmployeeRequestJson>
{
    public AddEmployeeUseCaseValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.Cpf).NotEmpty().WithMessage("CPF is required.")
            .Length(11).WithMessage("CPF must have 11 digits.");

        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");

        RuleFor(x => x.Salary).GreaterThan(0).WithMessage("Salary must be greater than zero.");

        RuleFor(x => x.PixKeyType).IsInEnum().WithMessage("Invalid PixKeyType.");

        RuleFor(x => x.PixKey).NotEmpty().WithMessage("PixKey is required.");

        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId is required.");

        RuleFor(x => x.DepartmentId).NotEmpty().WithMessage("DepartmentId is required.");

        RuleFor(x => x.PositionId).NotEmpty().WithMessage("PositionId is required.");
    }
}
