using Microsoft.EntityFrameworkCore;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly PeopleBankDbContext _context;

    public EmployeeRepository(PeopleBankDbContext context) => _context = context;

    public async Task<Employee?> GetByIdAsync(Guid id) =>
        await _context.Employees
            .Include(e => e.Company)
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Account)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Employee?> GetByCpfAsync(string cpf) =>
        await _context.Employees.FirstOrDefaultAsync(e => e.Cpf.Value == cpf);

    public async Task<IEnumerable<Employee>> GetByCompanyAsync(Guid companyId) =>
        await _context.Employees.Where(e => e.CompanyId == companyId && e.Active).ToListAsync();

    public async Task AddAsync(Employee employee) =>
        await _context.Employees.AddAsync(employee);

    public void Update(Employee employee) =>
        _context.Employees.Update(employee);
}