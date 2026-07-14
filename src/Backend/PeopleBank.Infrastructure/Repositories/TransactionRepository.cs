using Microsoft.EntityFrameworkCore;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly PeopleBankDbContext _context;

    public TransactionRepository(PeopleBankDbContext context) => _context = context;

    public async Task AddAsync(Transaction transaction) =>
        await _context.Transactions.AddAsync(transaction);

    public async Task<Transaction?> GetByIdempotencyKeyAsync(string key) =>
        await _context.Transactions.FirstOrDefaultAsync(t => t.IdempotencyKey == key);

    public async Task<IEnumerable<Transaction>> GetByAccountAsync(Guid accountId, int page = 1, int pageSize = 20) =>
        await _context.Transactions
            .Where(t => t.SourceAccountId == accountId || t.TargetAccountId == accountId)
            .Include(t => t.SourceAccount).ThenInclude(a => a.Employee)
            .Include(t => t.TargetAccount).ThenInclude(a => a.Employee)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
}