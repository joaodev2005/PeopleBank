using PeopleBank.Domain.Entities;

namespace PeopleBank.Domain.Interfaces.Repositories;

public interface IBenefitDefinitionRepository
{
    Task<BenefitDefinition?> GetByIdAsync(Guid id);
    Task<IEnumerable<BenefitDefinition>> GetByCompanyAsync(Guid companyId);
    Task AddAsync(BenefitDefinition benefitDefinition);
    void Update(BenefitDefinition benefitDefinition);
}
