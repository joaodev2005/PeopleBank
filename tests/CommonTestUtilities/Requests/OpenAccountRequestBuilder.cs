using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Enums;

namespace PeopleBank.CommonTestUtilities.Requests;

public class OpenAccountRequestBuilder
{
    public static OpenAccountRequestJson Build(
        Guid? employeeId = null,
        string? pixKey = null,
        PixKeyType pixType = PixKeyType.Email)
    {
        return new OpenAccountRequestJson
        {
            EmployeeId = employeeId ?? Guid.NewGuid(),
            PixKey = pixKey ?? "test@example.com",
            PixKeyType = pixType.ToString()
        };
    }
}