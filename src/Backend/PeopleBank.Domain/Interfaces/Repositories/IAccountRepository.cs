using PeopleBank.Domain.Entities;

namespace PeopleBank.Domain.Interfaces.Repositories;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account?> GetByEmployeeIdAsync(Guid employeeId);
    Task<Account?> GetByPixKeyAsync(string pixKey);
    Task AddAsync(Account account);
    void Update(Account account);
}
