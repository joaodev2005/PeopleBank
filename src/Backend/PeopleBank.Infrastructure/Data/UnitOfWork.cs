using PeopleBank.Domain.Interfaces.Services;

namespace PeopleBank.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly PeopleBankDbContext _context;

    public UnitOfWork(PeopleBankDbContext context) => _context = context;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync()
        => await _context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
        => await _context.Database.CommitTransactionAsync();

    public async Task RollbackTransactionAsync()
        => await _context.Database.RollbackTransactionAsync();
}