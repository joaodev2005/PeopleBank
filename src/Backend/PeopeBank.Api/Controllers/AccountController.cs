using Microsoft.AspNetCore.Mvc;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Application.UseCases.Account.GetStatement;
using PeopleBank.Application.UseCases.Account.OpenAccount;

namespace PeopleBank.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountController : ControllerBase
{
    private readonly IOpenAccountUseCase _openAccountUseCase;
    private readonly IGetStatementUseCase _getStatementUseCase;

    public AccountController(
        IOpenAccountUseCase openAccountUseCase,
        IGetStatementUseCase getStatementUseCase)
    {
        _openAccountUseCase = openAccountUseCase;
        _getStatementUseCase = getStatementUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseAccountJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Open([FromBody] OpenAccountRequestJson request)
    {
        var response = await _openAccountUseCase.Execute(request);
        return CreatedAtAction(nameof(GetStatement), new { id = response.Id }, response);
    }

    [HttpGet("{id}/statement")]
    [ProducesResponseType(typeof(ResponseStatementJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatement(Guid id)
    {
        var response = await _getStatementUseCase.Execute(id);
        return Ok(response);
    }
}