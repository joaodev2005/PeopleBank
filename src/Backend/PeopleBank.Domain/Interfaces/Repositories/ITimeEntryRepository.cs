using PeopleBank.Domain.Entities;

namespace PeopleBank.Domain.Interfaces.Repositories;

public interface ITimeEntryRepository
{
    Task AddAsync(TimeEntry timeEntry);
    Task<IEnumerable<TimeEntry>> GetByEmployeeAndMonthAsync(Guid employeeId, int month, int year);
    Task<TimeEntry?> GetLastEntryAsync(Guid employeeId);
}
