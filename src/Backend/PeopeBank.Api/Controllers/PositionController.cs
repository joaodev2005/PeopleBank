using Microsoft.AspNetCore.Mvc;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Application.UseCases.Position;

namespace PeopleBank.Api.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionController : ControllerBase
{
    private readonly ICreatePositionUseCase _createPositionUseCase;

    public PositionController(ICreatePositionUseCase createPositionUseCase)
    {
        _createPositionUseCase = createPositionUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponsePositionJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePositionRequestJson request)
    {
        var response = await _createPositionUseCase.Execute(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponsePositionJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        await Task.CompletedTask;
        return Ok();
    }
}