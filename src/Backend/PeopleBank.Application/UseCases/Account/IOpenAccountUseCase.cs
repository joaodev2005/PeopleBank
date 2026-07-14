using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Account;

public interface IOpenAccountUseCase
{
    Task<ResponseAccountJson> Execute(OpenAccountRequestJson request);
}
