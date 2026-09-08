using PeopleBank.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class AddProcessPayrollRequestJsonBuilder
{
    public static ProcessPayrollRequestJson Build() => new()
    {
        CompanyId = Guid.NewGuid(),
        Month = 8,
        Year = 2026
    };
}
