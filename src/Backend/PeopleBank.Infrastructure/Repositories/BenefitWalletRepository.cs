using Microsoft.EntityFrameworkCore;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Infrastructure.Repositories;

public class BenefitWalletRepository : IBenefitWalletRepository
{
    private readonly PeopleBankDbContext _context;

    public BenefitWalletRepository(PeopleBankDbContext context) => _context = context;

    public async Task<BenefitWallet?> GetByAccountAndCategoryAsync(Guid accountId, BenefitCategory category)
    {
        return await _context.BenefitWallets
            .Include(w => w.BenefitDefinition)
            .FirstOrDefaultAsync(w => w.AccountId == accountId
                                      && w.BenefitDefinition.Category == category
                                      && w.ExpirationDate > DateTime.UtcNow);
    }

    public async Task AddAsync(BenefitWallet wallet) =>
        await _context.BenefitWallets.AddAsync(wallet);

    public void Update(BenefitWallet wallet) =>
        _context.BenefitWallets.Update(wallet);
}