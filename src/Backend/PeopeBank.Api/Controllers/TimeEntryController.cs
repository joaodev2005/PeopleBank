using Microsoft.AspNetCore.Mvc;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Application.UseCases.TimeEntry;

namespace PeopleBank.Api.Controllers;

[ApiController]
[Route("api/time-entries")]
public class TimeEntryController : ControllerBase
{
    private readonly IRegisterTimeEntryUseCase _registerTimeEntryUseCase;

    public TimeEntryController(IRegisterTimeEntryUseCase registerTimeEntryUseCase)
    {
        _registerTimeEntryUseCase = registerTimeEntryUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseTimeEntryJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Register([FromBody] RegisterTimeEntryRequestJson request)
    {
        var response = await _registerTimeEntryUseCase.Execute(request);
        return Created(string.Empty, response);
    }
}