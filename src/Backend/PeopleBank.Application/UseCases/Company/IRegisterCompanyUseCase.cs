using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Company;

public interface IRegisterCompanyUseCase
{
    Task<ResponseCompanyJson> Execute(RegisterCompanyRequestJson request);
}
