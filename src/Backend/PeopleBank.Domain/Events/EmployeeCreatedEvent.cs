namespace PeopleBank.Domain.Events;

public record EmployeeCreatedEvent
{
    public Guid EmployeeId { get; init; }
    public string Name { get; init; }
    public string PixKey { get; init; }
    public string PixKeyType { get; init; } 
    public DateTime CreatedAt { get; init; }
}
