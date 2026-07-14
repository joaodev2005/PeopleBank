using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Benefits;

public interface IDefineBenefitUseCase
{
    Task<ResponseBenefitJson> Execute(DefineBenefitRequestJson request);
}
