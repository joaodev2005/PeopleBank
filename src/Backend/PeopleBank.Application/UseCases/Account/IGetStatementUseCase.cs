using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Account;

public interface IGetStatementUseCase
{
    Task<ResponseStatementJson> Execute(Guid accountId);
}
