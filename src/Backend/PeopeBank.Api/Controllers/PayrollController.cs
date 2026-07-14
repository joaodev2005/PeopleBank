using Microsoft.AspNetCore.Mvc;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Application.UseCases.Payroll;

namespace PeopleBank.Api.Controllers;

[ApiController]
[Route("api/payroll")]
public class PayrollController : ControllerBase
{
    private readonly IProcessPayrollUseCase _processPayrollUseCase;

    public PayrollController(IProcessPayrollUseCase processPayrollUseCase)
    {
        _processPayrollUseCase = processPayrollUseCase;
    }

    [HttpPost("process")]
    [ProducesResponseType(typeof(ResponsePayrollJson), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Process([FromBody] ProcessPayrollRequestJson request)
    {
        var response = await _processPayrollUseCase.Execute(request);
        return Accepted(response);
    }
}