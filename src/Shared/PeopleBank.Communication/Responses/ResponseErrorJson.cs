namespace PeopleBank.Communication.Responses;

public class ResponseErrorJson
{
    public List<string> Errors { get; set; } = new();
    public bool Success => Errors.Count == 0;

    public ResponseErrorJson(string errorMessage)
    {
        Errors = new List<string> { errorMessage };
    }

    public ResponseErrorJson(List<string> errorMessages)
    {
        Errors = errorMessages;
    }
}