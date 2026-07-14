using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Payroll;

public interface IProcessPayrollUseCase
{
    Task<ResponsePayrollJson> Execute(ProcessPayrollRequestJson request);
}
