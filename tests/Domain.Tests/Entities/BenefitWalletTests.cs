using FluentAssertions;
using PeopleBank.Domain.Entities;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Tests.Entities;

public class BenefitWalletTests
{
    [Fact]
    public void Credit_ValidAmount_ShouldIncreaseBalance()
    {
        var wallet = new BenefitWallet(Guid.NewGuid(), Guid.NewGuid(), 0, DateTime.UtcNow.AddDays(30));

        wallet.Credit(200);

        wallet.Balance.Should().Be(200);
    }

    [Fact]
    public void Debit_InsufficientBalance_ShouldThrow()
    {
        var wallet = new BenefitWallet(Guid.NewGuid(), Guid.NewGuid(), 50, DateTime.UtcNow.AddDays(30));

        Action act = () => wallet.Debit(100);

        act.Should().Throw<DomainException>();
    }
}