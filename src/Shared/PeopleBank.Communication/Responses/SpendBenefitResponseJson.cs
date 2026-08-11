namespace PeopleBank.Communication.Responses;

public class SpendBenefitResponseJson
{
    public Guid TransactionId { get; set; }
    public decimal NewBalance { get; set; }
    public string BenefitCategory { get; set; } = string.Empty;
}