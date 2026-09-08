using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;

namespace PeopleBank.CommonTestUtilities.Builders;

public class TimeEntryBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _employeeId = Guid.NewGuid();
    private DateTime _timestamp = DateTime.UtcNow;
    private TimeEntryType _type = TimeEntryType.ClockIn;

    public TimeEntryBuilder WithId(Guid id) { _id = id; return this; }
    public TimeEntryBuilder WithEmployeeId(Guid employeeId) { _employeeId = employeeId; return this; }
    public TimeEntryBuilder WithTimestamp(DateTime timestamp) { _timestamp = timestamp; return this; }
    public TimeEntryBuilder WithType(TimeEntryType type) { _type = type; return this; }

    public TimeEntry Build()
    {
        var entry = new TimeEntry(_employeeId, _timestamp, _type);
        var idProp = typeof(TimeEntry).GetProperty(nameof(TimeEntry.Id));
        idProp!.SetValue(entry, _id);
        return entry;
    }
}