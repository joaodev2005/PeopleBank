using Microsoft.EntityFrameworkCore;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly PeopleBankDbContext _context;

    public DepartmentRepository(PeopleBankDbContext context) => _context = context;

    public async Task<Department?> GetByIdAsync(Guid id) =>
        await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);

    public async Task AddAsync(Department department) =>
        await _context.Departments.AddAsync(department);
}