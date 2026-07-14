using Microsoft.AspNetCore.Mvc;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Application.UseCases.Department;

namespace PeopleBank.Api.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentController : ControllerBase
{
    private readonly ICreateDepartmentUseCase _createDepartmentUseCase;

    public DepartmentController(ICreateDepartmentUseCase createDepartmentUseCase)
    {
        _createDepartmentUseCase = createDepartmentUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseDepartmentJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequestJson request)
    {
        var response = await _createDepartmentUseCase.Execute(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDepartmentJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        await Task.CompletedTask;
        return Ok();
    }
}