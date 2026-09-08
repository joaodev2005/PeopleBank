using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Benefits.DefineBenefits;

public interface IDefineBenefitUseCase
{
    Task<ResponseBenefitJson> Execute(DefineBenefitRequestJson request);
}
