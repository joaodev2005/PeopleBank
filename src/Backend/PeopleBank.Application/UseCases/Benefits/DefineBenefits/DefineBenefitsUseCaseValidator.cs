using FluentValidation;
using PeopleBank.Communication.Requests;

namespace PeopleBank.Application.UseCases.Benefits.DefineBenefits;

public class DefineBenefitsUseCaseValidator : AbstractValidator<DefineBenefitRequestJson>
{
    public DefineBenefitsUseCaseValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Category).IsInEnum().WithMessage("Invalid BenefitCategory.");
        RuleFor(x => x.MonthlyAmount).GreaterThan(0).WithMessage("MonthlyAmount must be greater than zero.");
    }
}
