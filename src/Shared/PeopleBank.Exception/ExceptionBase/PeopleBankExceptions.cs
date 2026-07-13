using System.Net;

namespace PeopleBank.Exception.ExceptionBase;

public abstract class PeopleBankExceptions : System.Exception
{
    public abstract HttpStatusCode GetStatusCode();
    public abstract List<string> GetErrorMessages();
}
