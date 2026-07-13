namespace PeopleBank.Domain.Events;

public record PayrollRequestedEvent
{
    public Guid PayrollId { get; init; }
    public Guid CompanyId { get; init; }
    public int Month { get; init; }
    public int Year { get; init; }
    public DateTime RequestedAt { get; init; }
}
