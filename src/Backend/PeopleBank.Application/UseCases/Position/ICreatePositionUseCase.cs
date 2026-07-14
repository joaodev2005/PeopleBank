using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Position;

public interface ICreatePositionUseCase
{
    Task<ResponsePositionJson> Execute(CreatePositionRequestJson request);
}