using FluentAssertions;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Tests.Entities;

public class AccountTests
{
    private Account CreateAccount(decimal initialBalance = 0)
    {
        var account = new Account(Guid.NewGuid(), "test@email.com", PixKeyType.Email);
        typeof(Account).GetProperty(nameof(Account.Balance))!.SetValue(account, initialBalance);
        return account;
    }

    [Fact]
    public void Credit_ValidAmount_ShouldIncreaseBalance()
    {
        var account = CreateAccount();

        account.Credit(500);

        account.Balance.Should().Be(500);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Credit_InvalidAmount_ShouldThrow(decimal amount)
    {
        var account = CreateAccount();

        Action act = () => account.Credit(amount);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void InternalDebit_WithSufficientBalance_ShouldDecreaseBalance()
    {
        var account = CreateAccount(1000);

        account.InternalDebit(300);

        account.Balance.Should().Be(700);
    }

    [Fact]
    public void InternalDebit_WithInsufficientBalance_ShouldThrow()
    {
        var account = CreateAccount(100);

        Action act = () => account.InternalDebit(200);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void BlockAmount_ValidAmount_ShouldIncreaseBlockedBalance()
    {
        var account = CreateAccount(1000);

        account.BlockAmount(250);

        account.BlockedBalance.Should().Be(250);
        account.AvailableBalance.Should().Be(750);
    }
}