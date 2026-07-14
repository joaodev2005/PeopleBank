using Microsoft.EntityFrameworkCore;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Infrastructure.Repositories;

public class TimeEntryRepository : ITimeEntryRepository
{
    private readonly PeopleBankDbContext _context;

    public TimeEntryRepository(PeopleBankDbContext context) => _context = context;

    public async Task AddAsync(TimeEntry timeEntry) =>
        await _context.TimeEntries.AddAsync(timeEntry);

    public async Task<IEnumerable<TimeEntry>> GetByEmployeeAndMonthAsync(Guid employeeId, int month, int year) =>
        await _context.TimeEntries
            .Where(t => t.EmployeeId == employeeId
                        && t.Timestamp.Month == month
                        && t.Timestamp.Year == year)
            .OrderBy(t => t.Timestamp)
            .ToListAsync();

    public async Task<TimeEntry?> GetLastEntryAsync(Guid employeeId) =>
        await _context.TimeEntries
            .Where(t => t.EmployeeId == employeeId)
            .OrderByDescending(t => t.Timestamp)
            .FirstOrDefaultAsync();
}