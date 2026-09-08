using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;

namespace PeopleBank.CommonTestUtilities.Builders;

public class TransactionBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _sourceAccountId = Guid.NewGuid();
    private Guid? _targetAccountId = null;
    private decimal _amount = 100m;
    private TransactionStatus _status = TransactionStatus.Pending;
    private string _idempotencyKey = Guid.NewGuid().ToString();
    private string? _description = "Test transaction";

    public TransactionBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TransactionBuilder WithSourceAccountId(Guid sourceAccountId)
    {
        _sourceAccountId = sourceAccountId;
        return this;
    }

    public TransactionBuilder WithTargetAccountId(Guid? targetAccountId)
    {
        _targetAccountId = targetAccountId;
        return this;
    }

    public TransactionBuilder WithAmount(decimal amount)
    {
        _amount = amount;
        return this;
    }

    public TransactionBuilder WithStatus(TransactionStatus status)
    {
        _status = status;
        return this;
    }

    public TransactionBuilder WithIdempotencyKey(string key)
    {
        _idempotencyKey = key;
        return this;
    }

    public TransactionBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public Transaction Build()
    {
        var tx = new Transaction(_sourceAccountId, _targetAccountId, _amount, _idempotencyKey, _description);
        var idProp = typeof(Transaction).GetProperty(nameof(Transaction.Id));
        idProp!.SetValue(tx, _id);

        var statusProp = typeof(Transaction).GetProperty(nameof(Transaction.Status));
        statusProp!.SetValue(tx, _status);

        if (_status == TransactionStatus.Completed)
        {
            var processedAtProp = typeof(Transaction).GetProperty(nameof(Transaction.ProcessedAt));
            processedAtProp!.SetValue(tx, DateTime.UtcNow);
        }
        else if (_status == TransactionStatus.Failed)
        {
            var processedAtProp = typeof(Transaction).GetProperty(nameof(Transaction.ProcessedAt));
            processedAtProp!.SetValue(tx, DateTime.UtcNow);
            var errorProp = typeof(Transaction).GetProperty(nameof(Transaction.ErrorMessage));
            errorProp!.SetValue(tx, "Failed");
        }

        return tx;
    }
}