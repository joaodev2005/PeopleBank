using PeopleBank.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class AddAccountRequestJsonBuilder
{
    public static OpenAccountRequestJson Build() => new()
    {
        EmployeeId = Guid.NewGuid(),
        PixKey = "key123",
        PixKeyType = "CPF"
    };
}
