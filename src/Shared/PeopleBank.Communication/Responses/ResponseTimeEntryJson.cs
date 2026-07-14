namespace PeopleBank.Communication.Responses;

public class ResponseTimeEntryJson
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
