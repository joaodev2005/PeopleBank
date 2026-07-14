namespace PeopleBank.Communication.Responses;

public class ResponsePixJson
{
    public Guid TransactionId { get; set; }
    public string Status { get; set; } = "Pending";
    public string Message { get; set; } = "Pix request accepted and being processed.";
}
