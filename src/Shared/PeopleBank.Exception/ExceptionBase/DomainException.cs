using System.Net;

namespace PeopleBank.Exception.ExceptionBase;

public class DomainException : PeopleBankExceptions
{
    private readonly string _message;

    public DomainException(string message)
    {
        _message = message;
    }

    public override List<string> GetErrorMessages() => new() { _message };

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
}