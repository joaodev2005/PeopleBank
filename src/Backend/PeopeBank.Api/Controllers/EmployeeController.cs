using Microsoft.AspNetCore.Mvc;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Application.UseCases.Employee;

namespace PeopleBank.Api.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeeController : ControllerBase
{
    private readonly IAddEmployeeUseCase _addEmployeeUseCase;

    public EmployeeController(IAddEmployeeUseCase addEmployeeUseCase)
    {
        _addEmployeeUseCase = addEmployeeUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseEmployeeJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Add([FromBody] AddEmployeeRequestJson request)
    {
        var response = await _addEmployeeUseCase.Execute(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseEmployeeJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        await Task.CompletedTask;
        return Ok();
    }
}