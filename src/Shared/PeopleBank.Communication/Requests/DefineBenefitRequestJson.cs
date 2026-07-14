using PeopleBank.Domain.Enums;

namespace PeopleBank.Communication.Requests;

public class DefineBenefitRequestJson
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public BenefitCategory Category { get; set; }
    public decimal MonthlyAmount { get; set; }
}
