using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;

namespace PeopleBank.CommonTestUtilities.Builders;

public class AccountBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _employeeId = Guid.NewGuid();
    private decimal _balance = 1000m;
    private decimal _blockedBalance = 0m;
    private PixKeyType _pixKeyType = PixKeyType.CPF;
    private string _pixKey = "12345678901";
    private bool _active = true;

    public AccountBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public AccountBuilder WithEmployeeId(Guid employeeId)
    {
        _employeeId = employeeId;
        return this;
    }

    public AccountBuilder WithBalance(decimal balance)
    {
        _balance = balance;
        return this;
    }

    public AccountBuilder WithBlockedBalance(decimal blockedBalance)
    {
        _blockedBalance = blockedBalance;
        return this;
    }

    public AccountBuilder WithPixKey(string pixKey, PixKeyType pixKeyType)
    {
        _pixKey = pixKey;
        _pixKeyType = pixKeyType;
        return this;
    }

    public AccountBuilder Inactive()
    {
        _active = false;
        return this;
    }

    public Account Build()
    {
        var account = new Account(_employeeId, _pixKey, _pixKeyType);

        var idProp = typeof(Account).GetProperty(nameof(Account.Id));
        idProp!.SetValue(account, _id);

        var balProp = typeof(Account).GetProperty(nameof(Account.Balance));
        balProp!.SetValue(account, _balance);

        var blockedProp = typeof(Account).GetProperty(nameof(Account.BlockedBalance));
        blockedProp!.SetValue(account, _blockedBalance);

        var activeProp = typeof(Account).GetProperty(nameof(Account.Active));
        activeProp!.SetValue(account, _active);

        return account;
    }
}