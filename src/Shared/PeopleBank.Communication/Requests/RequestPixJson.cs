namespace PeopleBank.Communication.Requests;

public class RequestPixJson
{
    public Guid SourceAccountId { get; set; }
    public string TargetPixKey { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
}