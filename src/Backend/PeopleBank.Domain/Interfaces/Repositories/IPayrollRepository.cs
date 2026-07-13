using PeopleBank.Domain.Entities;

namespace PeopleBank.Domain.Interfaces.Repositories;

public interface IPayrollRepository
{
    Task<Payroll?> GetByIdAsync(Guid id);
    Task<IEnumerable<Payroll>> GetByCompanyAndPeriodAsync(Guid companyId, int month, int year);
    Task AddAsync(Payroll payroll);
    void Update(Payroll payroll);

}
