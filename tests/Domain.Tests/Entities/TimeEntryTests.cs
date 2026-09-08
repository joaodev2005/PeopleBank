using FluentAssertions;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;

namespace PeopleBank.Domain.Tests.Entities;

public class TimeEntryTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var employeeId = Guid.NewGuid();
        var timestamp = new DateTime(2026, 8, 22, 8, 0, 0, DateTimeKind.Utc);
        var type = TimeEntryType.ClockIn;

        var entry = new TimeEntry(employeeId, timestamp, type);

        entry.Id.Should().NotBe(Guid.Empty);
        entry.EmployeeId.Should().Be(employeeId);
        entry.Timestamp.Should().Be(timestamp);
        entry.Type.Should().Be(type);
        entry.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}