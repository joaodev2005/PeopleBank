using Microsoft.EntityFrameworkCore;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Infrastructure.Repositories;

public class PayrollRepository : IPayrollRepository
{
    private readonly PeopleBankDbContext _context;

    public PayrollRepository(PeopleBankDbContext context) => _context = context;

    public async Task<Payroll?> GetByIdAsync(Guid id) =>
        await _context.Payrolls
            .Include(p => p.Payslips)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Payroll>> GetByCompanyAndPeriodAsync(Guid companyId, int month, int year) =>
        await _context.Payrolls
            .Where(p => p.CompanyId == companyId && p.Month == month && p.Year == year)
            .ToListAsync();

    public async Task AddAsync(Payroll payroll) =>
        await _context.Payrolls.AddAsync(payroll);

    public void Update(Payroll payroll) =>
        _context.Payrolls.Update(payroll);
}