namespace PeopleBank.Domain.Events;

public record PixRequestedEvent
{
    public Guid TransactionId { get; init; }
    public Guid SourceAccountId { get; init; }
    public Guid TargetAccountId { get; init; }
    public decimal Amount { get; init; }
    public string? IdempotencyKey { get; init; }
    public string? Description { get; init; }
    public DateTime RequestedAt { get; init; }
}
