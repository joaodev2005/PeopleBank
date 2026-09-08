using PeopleBank.Domain.Entities;

namespace PeopleBank.CommonTestUtilities.Builders;

public class BenefitWalletBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _accountId = Guid.NewGuid();
    private Guid _benefitDefinitionId = Guid.NewGuid();
    private decimal _balance = 300m;
    private DateTime _expirationDate = DateTime.UtcNow.AddMonths(1);

    public BenefitWalletBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public BenefitWalletBuilder WithAccountId(Guid accountId)
    {
        _accountId = accountId;
        return this;
    }

    public BenefitWalletBuilder WithBenefitDefinitionId(Guid benefitDefinitionId)
    {
        _benefitDefinitionId = benefitDefinitionId;
        return this;
    }

    public BenefitWalletBuilder WithBalance(decimal balance)
    {
        _balance = balance;
        return this;
    }

    public BenefitWalletBuilder WithExpirationDate(DateTime expirationDate)
    {
        _expirationDate = expirationDate;
        return this;
    }

    public BenefitWallet Build()
    {
        var wallet = new BenefitWallet(_accountId, _benefitDefinitionId, _balance, _expirationDate);
        var idProp = typeof(BenefitWallet).GetProperty(nameof(BenefitWallet.Id));
        idProp!.SetValue(wallet, _id);
        return wallet;
    }
}