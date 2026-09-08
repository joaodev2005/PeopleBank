using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Enums;

namespace PeopleBank.CommonTestUtilities.Requests;

public class RegisterTimeEntryRequestBuilder
{
    public static RegisterTimeEntryRequestJson Build(
        Guid? employeeId = null,
        TimeEntryType type = TimeEntryType.ClockIn,
        DateTime? timestamp = null)
    {
        return new RegisterTimeEntryRequestJson
        {
            EmployeeId = employeeId ?? Guid.NewGuid(),
            Timestamp = timestamp ?? DateTime.UtcNow,
            Type = type.ToString()
        };
    }
}