using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Enums;

namespace PeopleBank.CommonTestUtilities.Requests;

public class DefineBenefitRequestBuilder
{
    public static DefineBenefitRequestJson Build(
        Guid? companyId = null,
        string name = "VR",
        BenefitCategory category = BenefitCategory.Meal,
        decimal monthlyAmount = 800)
    {
        return new DefineBenefitRequestJson
        {
            CompanyId = companyId ?? Guid.NewGuid(),
            Name = name,
            Category = category,
            MonthlyAmount = monthlyAmount
        };
    }
}