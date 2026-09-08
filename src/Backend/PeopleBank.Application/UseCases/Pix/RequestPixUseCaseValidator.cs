using FluentValidation;
using PeopleBank.Communication.Requests;

namespace PeopleBank.Application.UseCases.Pix;

public class RequestPixUseCaseValidator : AbstractValidator<RequestPixJson>
{
    public RequestPixUseCaseValidator()
    {
        RuleFor(x => x.SourceAccountId).NotEmpty().WithMessage("SourceAccountId is required.");
        RuleFor(x => x.TargetPixKey).NotEmpty().WithMessage("TargetPixKey is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.IdempotencyKey).NotEmpty().WithMessage("IdempotencyKey is required.");
    }
}
