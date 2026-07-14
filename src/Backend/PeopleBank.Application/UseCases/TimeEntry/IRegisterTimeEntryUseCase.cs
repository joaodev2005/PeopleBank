using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.TimeEntry;

public interface IRegisterTimeEntryUseCase
{
    Task<ResponseTimeEntryJson> Execute(RegisterTimeEntryRequestJson request);
}
