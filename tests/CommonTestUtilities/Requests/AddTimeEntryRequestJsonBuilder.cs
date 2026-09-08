using PeopleBank.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class AddTimeEntryRequestJsonBuilder
{
    public static RegisterTimeEntryRequestJson Build() => new()
    {
        EmployeeId = Guid.NewGuid(),
        Timestamp = DateTime.UtcNow,
        Type = "ClockIn"
    };
}
