using FluentValidation;
using PeopleBank.Communication.Requests;

namespace PeopleBank.Application.UseCases.Payroll;

public class ProcessPayrollUseCaseValidator : AbstractValidator<ProcessPayrollRequestJson>
{
    public ProcessPayrollUseCaseValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId is required.");
        RuleFor(x => x.Month).InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");
        RuleFor(x => x.Year).GreaterThan(1900).WithMessage("Year must be valid.");
    }
}
