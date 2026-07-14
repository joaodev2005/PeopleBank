using Microsoft.AspNetCore.Mvc;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Application.UseCases.Benefits;

namespace PeopleBank.Api.Controllers;

[ApiController]
[Route("api/benefits")]
public class BenefitsController : ControllerBase
{
    private readonly IDefineBenefitUseCase _defineBenefitUseCase;

    public BenefitsController(IDefineBenefitUseCase defineBenefitUseCase)
    {
        _defineBenefitUseCase = defineBenefitUseCase;
    }

    [HttpPost("definitions")]
    [ProducesResponseType(typeof(ResponseBenefitJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DefineBenefit([FromBody] DefineBenefitRequestJson request)
    {
        var response = await _defineBenefitUseCase.Execute(request);
        return Created(string.Empty, response);
    }
}