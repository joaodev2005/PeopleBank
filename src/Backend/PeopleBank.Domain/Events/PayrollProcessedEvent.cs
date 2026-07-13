namespace PeopleBank.Domain.Events;

public record PayrollProcessedEvent
{
    public Guid PayrollId { get; init; }
    public Guid CompanyId { get; init; }
    public int Month { get; init; }
    public int Year { get; init; }
    public int EmployeesPaid { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime ProcessedAt { get; init; }
}
