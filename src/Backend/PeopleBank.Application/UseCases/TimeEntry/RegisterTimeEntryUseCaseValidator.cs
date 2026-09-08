using FluentValidation;
using PeopleBank.Communication.Requests;

namespace PeopleBank.Application.UseCases.TimeEntry;

public class RegisterTimeEntryUseCaseValidator : AbstractValidator<RegisterTimeEntryRequestJson>
{
    public RegisterTimeEntryUseCaseValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("EmployeeId is required.");
        RuleFor(x => x.Timestamp).NotEmpty().WithMessage("Timestamp is required.");
        RuleFor(x => x.Type).NotEmpty().WithMessage("Type is required.")
            .Must(v => v == "ClockIn" || v == "ClockOut").WithMessage("Type must be ClockIn or ClockOut.");
    }
}
