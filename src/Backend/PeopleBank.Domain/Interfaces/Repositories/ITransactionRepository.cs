using PeopleBank.Domain.Entities;

namespace PeopleBank.Domain.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
    Task<Transaction?> GetByIdempotencyKeyAsync(string key);
    Task<IEnumerable<Transaction>> GetByAccountAsync(Guid accountId, int page = 1, int pageSize = 20);
}
