using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Department;

public interface ICreateDepartmentUseCase
{
    Task<ResponseDepartmentJson> Execute(CreateDepartmentRequestJson request);
}