using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Employee;

public interface IAddEmployeeUseCase
{
    Task<ResponseEmployeeJson> Execute(AddEmployeeRequestJson request);
}
