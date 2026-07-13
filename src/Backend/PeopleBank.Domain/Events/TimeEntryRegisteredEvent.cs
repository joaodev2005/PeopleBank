namespace PeopleBank.Domain.Events;

public record TimeEntryRegisteredEvent
{
    public Guid TimeEntryId { get; init; }
    public Guid EmployeeId { get; init; }
    public DateTime Timestamp { get; init; }
    public string Type { get; init; } 
}
