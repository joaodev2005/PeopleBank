using PeopleBank.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class AddBenefitsRequestJsonBuilder
{
    public static DefineBenefitRequestJson Build() => new()
    {
        CompanyId = Guid.NewGuid(),
        Name = "VR",
        Category = PeopleBank.Domain.Enums.BenefitCategory.Meal,
        MonthlyAmount = 500
    };
}
