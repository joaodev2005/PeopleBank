using PeopleBank.Domain.Enums;

namespace PeopleBank.Communication.Requests;

public class SpendBenefitRequestJson
{
    public Guid AccountId { get; set; }
    public BenefitCategory BenefitCategory { get; set; }
    public decimal Amount { get; set; }
    public EstablishmentCategory EstablishmentCategory { get; set; }
    public string? Description { get; set; }
    public string? IdempotencyKey { get; set; }
}