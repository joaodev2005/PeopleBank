using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Enums;

namespace CommonTestUtilities.Requests;

public class AddSpendBenefitRequestJsonBuilder
{
    public static SpendBenefitRequestJson Build() => new()
    {
        AccountId = Guid.NewGuid(),
        BenefitCategory = BenefitCategory.Meal,
        Amount = 100,
        EstablishmentCategory = EstablishmentCategory.Restaurant
    };
}
