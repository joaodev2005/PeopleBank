using Microsoft.AspNetCore.Mvc;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Application.UseCases.Benefits.DefineBenefits;
using PeopleBank.Application.UseCases.Benefits.SpendBenefits;

namespace PeopleBank.Api.Controllers;

[ApiController]
[Route("api/benefits")]
public class BenefitsController : ControllerBase
{
    private readonly IDefineBenefitUseCase _defineBenefitUseCase;
    private readonly ISpendBenefitUseCase _spendBenefitUseCase;

    public BenefitsController(IDefineBenefitUseCase defineBenefitUseCase, ISpendBenefitUseCase spendBenefitUseCase)
    {
        _defineBenefitUseCase = defineBenefitUseCase;
        _spendBenefitUseCase = spendBenefitUseCase;
    }

    [HttpPost("definitions")]
    [ProducesResponseType(typeof(ResponseBenefitJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DefineBenefit([FromBody] DefineBenefitRequestJson request)
    {
        var response = await _defineBenefitUseCase.Execute(request);
        return Created(string.Empty, response);
    }

    [HttpPost("spend")]
    [ProducesResponseType(typeof(SpendBenefitResponseJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SpendBenefit([FromBody] SpendBenefitRequestJson request)
    {
        var response = await _spendBenefitUseCase.Execute(request);
        return Ok(response);
    }
}