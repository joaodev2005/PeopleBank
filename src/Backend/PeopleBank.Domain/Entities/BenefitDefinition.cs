using PeopleBank.Domain.Enums;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Entities;

public class BenefitDefinition
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public BenefitCategory Category { get; private set; }
    public decimal MonthlyAmount { get; private set; }
    public bool Active { get; private set; }

    public Company Company { get; private set; }

    private BenefitDefinition() { }

    public BenefitDefinition(Guid companyId, string name, BenefitCategory category, decimal monthlyAmount)
    {
        if (monthlyAmount <= 0)
            throw new DomainException("Monthly amount must be greater than zero.");

        Id = Guid.NewGuid();
        CompanyId = companyId;
        Name = name;
        Category = category;
        MonthlyAmount = monthlyAmount;
        Active = true;
    }
}