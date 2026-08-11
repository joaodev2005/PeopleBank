using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Entities;

public class BenefitWallet
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid BenefitDefinitionId { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime ExpirationDate { get; private set; }

    public Account Account { get; private set; }
    public BenefitDefinition BenefitDefinition { get; private set; }

    private BenefitWallet() { }

    public BenefitWallet(Guid accountId, Guid benefitDefinitionId, decimal initialBalance, DateTime expirationDate)
    {
        if (initialBalance < 0)
            throw new DomainException("Initial balance cannot be negative.");

        Id = Guid.NewGuid();
        AccountId = accountId;
        BenefitDefinitionId = benefitDefinitionId;
        Balance = initialBalance;
        ExpirationDate = expirationDate;
    }

    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Debit amount must be greater than zero.");

        if (Balance < amount)
            throw new DomainException("Insufficient benefit balance.");

        Balance -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Credit amount must be greater than zero.");

        Balance += amount;
    }
}