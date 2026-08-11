using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;

namespace PeopleBank.Domain.Interfaces.Repositories;

public interface IBenefitWalletRepository
{
    Task<BenefitWallet?> GetByAccountAndCategoryAsync(Guid accountId, BenefitCategory category);
    Task AddAsync(BenefitWallet wallet);
    void Update(BenefitWallet wallet);
}