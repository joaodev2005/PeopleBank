using PeopleBank.Domain.Entities;

namespace PeopleBank.Domain.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id);
    Task<Employee?> GetByCpfAsync(string cpf);
    Task<IEnumerable<Employee>> GetByCompanyAsync(Guid companyId);
    Task AddAsync(Employee employee);
    void Update(Employee employee);
}
