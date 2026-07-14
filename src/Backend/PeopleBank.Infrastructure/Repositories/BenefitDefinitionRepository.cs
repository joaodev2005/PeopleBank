using Microsoft.EntityFrameworkCore;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Infrastructure.Repositories;

public class BenefitDefinitionRepository : IBenefitDefinitionRepository
{
    private readonly PeopleBankDbContext _context;

    public BenefitDefinitionRepository(PeopleBankDbContext context) => _context = context;

    public async Task<BenefitDefinition?> GetByIdAsync(Guid id) =>
        await _context.BenefitDefinitions.FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<BenefitDefinition>> GetByCompanyAsync(Guid companyId) =>
        await _context.BenefitDefinitions.Where(b => b.CompanyId == companyId && b.Active).ToListAsync();

    public async Task AddAsync(BenefitDefinition benefitDefinition) =>
        await _context.BenefitDefinitions.AddAsync(benefitDefinition);

    public void Update(BenefitDefinition benefitDefinition) =>
        _context.BenefitDefinitions.Update(benefitDefinition);
}