using PeopleBank.Domain.Entities;

namespace PeopleBank.Domain.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id);
    Task<Company?> GetByCnpjAsync(string cnpj);
    Task AddAsync(Company company);
    void Update(Company company);
}
