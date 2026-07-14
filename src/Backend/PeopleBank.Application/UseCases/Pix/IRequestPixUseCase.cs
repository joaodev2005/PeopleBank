using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;

namespace PeopleBank.Application.UseCases.Pix;

public interface IRequestPixUseCase
{
    Task<ResponsePixJson> Execute(RequestPixJson request);
}
