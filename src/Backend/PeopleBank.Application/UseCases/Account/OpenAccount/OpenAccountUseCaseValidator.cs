using FluentValidation;
using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Enums;

namespace PeopleBank.Application.UseCases.Account.OpenAccount;

public class OpenAccountUseCaseValidator : AbstractValidator<OpenAccountRequestJson>
{
    public OpenAccountUseCaseValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("EmployeeId is required.");
        RuleFor(x => x.PixKey).NotEmpty().WithMessage("PixKey is required.");
        RuleFor(x => x.PixKeyType).NotEmpty().WithMessage("PixKeyType is required.")
            .Must(v => Enum.TryParse<PixKeyType>(v, true, out _)).WithMessage("Invalid PixKeyType.");
    }
}
