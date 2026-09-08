using FluentAssertions;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;

namespace PeopleBank.Domain.Tests.Entities;

public class TransactionTests
{
    private Transaction CreateTransaction()
    {
        return new Transaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            100m,
            "idem-key",
            "Pix Transfer"
        );
    }

    [Fact]
    public void MarkAsCompleted_ShouldSetStatusAndDate()
    {
        var transaction = CreateTransaction();

        transaction.MarkAsCompleted();

        transaction.Status.Should().Be(TransactionStatus.Completed);
        transaction.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsFailed_ShouldSetStatusAndError()
    {
        var transaction = CreateTransaction();

        transaction.MarkAsFailed("Saldo insuficiente");

        transaction.Status.Should().Be(TransactionStatus.Failed);
        transaction.ErrorMessage.Should().Be("Saldo insuficiente");
        transaction.ProcessedAt.Should().NotBeNull();
    }
}