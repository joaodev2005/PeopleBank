using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Benefits.SpendBenefits;

public interface ISpendBenefitUseCase
{
    Task<SpendBenefitResponseJson> Execute(SpendBenefitRequestJson request);
}