using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Account.GetStatement;

public interface IGetStatementUseCase
{
    Task<ResponseStatementJson> Execute(Guid accountId);
}
