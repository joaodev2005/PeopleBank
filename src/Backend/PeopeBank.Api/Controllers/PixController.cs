using Microsoft.AspNetCore.Mvc;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Application.UseCases.Pix;

namespace PeopleBank.Api.Controllers;

[ApiController]
[Route("api/pix")]
public class PixController : ControllerBase
{
    private readonly IRequestPixUseCase _requestPixUseCase;

    public PixController(IRequestPixUseCase requestPixUseCase)
    {
        _requestPixUseCase = requestPixUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponsePixJson), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestPix(
        [FromBody] RequestPixJson request,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        request.IdempotencyKey = idempotencyKey;
        var response = await _requestPixUseCase.Execute(request);
        return Accepted(response);
    }
}