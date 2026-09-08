using FluentValidation;
using PeopleBank.Communication.Requests;

namespace PeopleBank.Application.UseCases.Company;

public class RegisterCompanyUseCaseValidator : AbstractValidator<RegisterCompanyRequestJson>
{
    public RegisterCompanyUseCaseValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Company name is required.");
        RuleFor(x => x.Cnpj).NotEmpty().WithMessage("CNPJ is required.")
            .Length(14).WithMessage("CNPJ must have 14 digits.");
    }
}
