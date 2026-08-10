using PeopleBank.Domain.Enums;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Entities;

public class Account
{
    public Guid Id { get; private set; }
    public Guid EmployeeId { get; private set; }
    public decimal Balance { get; private set; }
    public decimal BlockedBalance { get; private set; }
    public decimal AvailableBalance => Balance - BlockedBalance;
    public PixKeyType PixKeyType { get; private set; }
    public string PixKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool Active { get; private set; }

    public Employee Employee { get; private set; }
    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    private readonly List<BenefitWallet> _benefitWallets = new();
    public IReadOnlyCollection<BenefitWallet> BenefitWallets => _benefitWallets.AsReadOnly();

    private Account() { }

    public Account(Guid employeeId, string pixKey, PixKeyType pixKeyType)
    {
        Id = Guid.NewGuid();
        EmployeeId = employeeId;
        PixKey = pixKey;
        PixKeyType = pixKeyType;
        Balance = 0; 
        BlockedBalance = 0;
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public void BlockAmount(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Block amount must be greater than zero.");

        if (AvailableBalance < amount)
            throw new DomainException(ResourceMessagesException.INSUFFICIENT_BALANCE);

        BlockedBalance += amount;
    }

    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Debit amount must be greater than zero.");

        if (BlockedBalance < amount)
            throw new DomainException("Insufficient blocked balance.");

        Balance -= amount;
        BlockedBalance -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Credit amount must be greater than zero.");

        Balance += amount;
    }

    public void AddTransaction(Transaction transaction)
    {
        _transactions.Add(transaction);
    }

    public void AddBenefitWallet(BenefitWallet wallet)
    {
        _benefitWallets.Add(wallet);
    }

    public void InternalDebit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Debit amount must be greater than zero.");

        if (Balance < amount)
            throw new DomainException("Insufficient balance.");

        Balance -= amount;
    }
}