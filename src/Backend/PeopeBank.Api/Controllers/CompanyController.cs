using Microsoft.AspNetCore.Mvc;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Application.UseCases.Company;

namespace PeopleBank.Api.Controllers;

[ApiController]
[Route("api/companies")]
public class CompanyController : ControllerBase
{
    private readonly IRegisterCompanyUseCase _registerCompanyUseCase;

    public CompanyController(IRegisterCompanyUseCase registerCompanyUseCase)
    {
        _registerCompanyUseCase = registerCompanyUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseCompanyJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCompanyRequestJson request)
    {
        var response = await _registerCompanyUseCase.Execute(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseCompanyJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        await Task.CompletedTask;
        return Ok();
    }
}