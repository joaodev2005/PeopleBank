using System.Net;

namespace PeopleBank.Exception.ExceptionBase;

public class NotFoundException : PeopleBankExceptions
{
    private readonly string _message;

    public NotFoundException(string message)
    {
        _message = message;
    }

    public override List<string> GetErrorMessages() => new() { _message };

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.NotFound;
}