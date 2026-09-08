using FluentValidation;
using PeopleBank.Communication.Requests;

namespace PeopleBank.Application.UseCases.Benefits.SpendBenefits;

public class SpendBenefitUseCaseValidator : AbstractValidator<SpendBenefitRequestJson>
{
    public SpendBenefitUseCaseValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty().WithMessage("AccountId is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.BenefitCategory).IsInEnum().WithMessage("Invalid BenefitCategory.");
        RuleFor(x => x.EstablishmentCategory).IsInEnum().WithMessage("Invalid EstablishmentCategory.");
        RuleFor(x => x.IdempotencyKey).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.IdempotencyKey));
    }
}
