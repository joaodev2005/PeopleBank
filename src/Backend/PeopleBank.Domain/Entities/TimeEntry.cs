using PeopleBank.Domain.Enums;

namespace PeopleBank.Domain.Entities;

public class TimeEntry
{
    public Guid Id { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public TimeEntryType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Employee Employee { get; private set; }

    private TimeEntry() { }

    public TimeEntry(Guid employeeId, DateTime timestamp, TimeEntryType type)
    {
        Id = Guid.NewGuid();
        EmployeeId = employeeId;
        Timestamp = timestamp;
        Type = type;
        CreatedAt = DateTime.UtcNow;
    }
}