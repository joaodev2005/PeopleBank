using Microsoft.EntityFrameworkCore;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly PeopleBankDbContext _context;

    public AccountRepository(PeopleBankDbContext context) => _context = context;

    public async Task<Account?> GetByIdAsync(Guid id) =>
        await _context.Accounts
            .Include(a => a.Employee)
            .Include(a => a.Transactions)
            .Include(a => a.BenefitWallets)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Account?> GetByEmployeeIdAsync(Guid employeeId) =>
        await _context.Accounts.FirstOrDefaultAsync(a => a.EmployeeId == employeeId);

    public async Task<Account?> GetByPixKeyAsync(string pixKey) =>
        await _context.Accounts.FirstOrDefaultAsync(a => a.PixKey == pixKey);

    public async Task AddAsync(Account account) =>
        await _context.Accounts.AddAsync(account);

    public void Update(Account account) =>
        _context.Accounts.Update(account);
}