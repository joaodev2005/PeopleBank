using Microsoft.EntityFrameworkCore;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly PeopleBankDbContext _context;

    public CompanyRepository(PeopleBankDbContext context) => _context = context;

    public async Task<Company?> GetByIdAsync(Guid id) =>
        await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Company?> GetByCnpjAsync(string cnpj) =>
    (await _context.Companies.ToListAsync())
        .FirstOrDefault(c => c.Cnpj.Value == cnpj);

    public async Task AddAsync(Company company) =>
        await _context.Companies.AddAsync(company);

    public void Update(Company company) =>
        _context.Companies.Update(company);
}