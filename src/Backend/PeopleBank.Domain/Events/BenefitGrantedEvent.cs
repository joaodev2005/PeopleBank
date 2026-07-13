namespace PeopleBank.Domain.Events;

public record BenefitGrantedEvent
{
    public Guid BenefitWalletId { get; init; }
    public Guid AccountId { get; init; }
    public Guid BenefitDefinitionId { get; init; }
    public decimal Amount { get; init; }
    public DateTime GrantedAt { get; init; }
}
