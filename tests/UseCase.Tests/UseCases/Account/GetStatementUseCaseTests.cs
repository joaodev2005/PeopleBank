using CommonTestUtilities.Mocks;
using FluentAssertions;
using Moq;
using PeopleBank.Application.UseCases.Account.GetStatement;
using PeopleBank.CommonTestUtilities.Builders;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.UseCase.Tests.Application.Account;

public class GetStatementUseCaseTests
{
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly Mock<ITransactionRepository> _transactionRepoMock;
    private readonly GetStatementUseCase _useCase;

    public GetStatementUseCaseTests()
    {
        _accountRepoMock = RepositoryMocks.CreateAccountRepository();
        _transactionRepoMock = RepositoryMocks.CreateTransactionRepository();
        _useCase = new GetStatementUseCase(_accountRepoMock.Object, _transactionRepoMock.Object);
    }

    [Fact]
    public async Task Execute_AccountNotFound_ShouldThrow()
    {
        _accountRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                        .ReturnsAsync((PeopleBank.Domain.Entities.Account?)null);

        Func<Task> act = () => _useCase.Execute(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Execute_Valid_ShouldReturnStatement()
    {
        var account = new AccountBuilder().WithBalance(1000).Build();
        var transactions = new List<Transaction>
        {
            new Transaction(account.Id, null, 500, "idem-1", "Salary"),
            new Transaction(account.Id, Guid.NewGuid(), 100, "idem-2", "Pix Transfer")
        };
        _accountRepoMock.Setup(r => r.GetByIdAsync(account.Id))
                        .ReturnsAsync(account);
        _transactionRepoMock.Setup(r => r.GetByAccountAsync(account.Id, 1, 50))
                            .ReturnsAsync(transactions);

        var result = await _useCase.Execute(account.Id);

        result.Should().NotBeNull();
        result.Balance.Should().Be(1000);
        result.Transactions.Should().HaveCount(2);
    }
}