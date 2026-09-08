using FluentAssertions;
using Moq;
using PeopleBank.Application.UseCases.Pix;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception.ExceptionBase;
using PeopleBank.CommonTestUtilities.Builders;
using Xunit;
using AccountEntity = PeopleBank.Domain.Entities.Account;
using PixRequestedEventEntity = PeopleBank.Domain.Events.PixRequestedEvent;

namespace PeopleBank.UseCase.Tests.UseCases.Pix;

public class RequestPixUseCaseTests
{
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly Mock<ITransactionRepository> _txRepo = new();
    private readonly Mock<IRedisService> _redis = new();
    private readonly Mock<IKafkaProducer> _kafka = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private RequestPixUseCase Create() => new(_accountRepo.Object, _txRepo.Object, _redis.Object, _kafka.Object, _uow.Object);

    [Fact]
    public async Task ShouldSucceed_WhenValid()
    {
        var source = new AccountBuilder().WithId(Guid.NewGuid()).WithBalance(1000).Build();
        var target = new AccountBuilder().WithId(Guid.NewGuid()).WithBalance(0).WithPixKey("target-key", PixKeyType.CPF).Build();
        var request = new RequestPixJson
        {
            SourceAccountId = source.Id,
            TargetPixKey = "target-key",
            Amount = 100,
            IdempotencyKey = "idem-1"
        };

        _redis.Setup(r => r.GetIdempotentResponseAsync<ResponsePixJson>(request.IdempotencyKey)).ReturnsAsync((ResponsePixJson?)null);
        _accountRepo.Setup(r => r.GetByIdAsync(source.Id)).ReturnsAsync((AccountEntity)source);
        _accountRepo.Setup(r => r.GetByPixKeyAsync("target-key")).ReturnsAsync((AccountEntity)target);
        _redis.Setup(r => r.AcquireLockAsync(It.IsAny<string>(), It.IsAny<decimal>())).ReturnsAsync(true);
        _txRepo.Setup(r => r.AddAsync(It.IsAny<Transaction>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _kafka.Setup(k => k.PublishAsync("pix-requested", It.IsAny<string>(), It.IsAny<PixRequestedEventEntity>())).Returns(Task.CompletedTask);
        _redis.Setup(r => r.SetIdempotentResponseAsync(It.IsAny<string>(), It.IsAny<ResponsePixJson>())).Returns(Task.CompletedTask);

        var useCase = Create();
        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Status.Should().Be("Pending");
        _redis.Verify(r => r.ReleaseLockAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ShouldReturnCached_WhenIdempotentExists()
    {
        var cached = new ResponsePixJson { TransactionId = Guid.NewGuid(), Status = "Pending" };
        var request = new RequestPixJson { IdempotencyKey = "idem-1", SourceAccountId = Guid.NewGuid(), TargetPixKey = "k", Amount = 10 };
        _redis.Setup(r => r.GetIdempotentResponseAsync<ResponsePixJson>(request.IdempotencyKey)).ReturnsAsync(cached);

        var useCase = Create();
        var result = await useCase.Execute(request);

        result.Should().BeSameAs(cached);
    }

    [Fact]
    public async Task ShouldThrow_WhenSourceAccountNotFound()
    {
        var request = new RequestPixJson { SourceAccountId = Guid.NewGuid(), TargetPixKey = "k", Amount = 10, IdempotencyKey = "idem" };
        _redis.Setup(r => r.GetIdempotentResponseAsync<ResponsePixJson>(request.IdempotencyKey)).ReturnsAsync((ResponsePixJson?)null);
        _accountRepo.Setup(r => r.GetByIdAsync(request.SourceAccountId)).ReturnsAsync((AccountEntity?)null);

        var useCase = Create();
        await Assert.ThrowsAsync<NotFoundException>(() => useCase.Execute(request));
    }

    [Fact]
    public async Task ShouldThrow_WhenTargetAccountNotFound()
    {
        var source = new AccountBuilder().Build();
        var request = new RequestPixJson { SourceAccountId = source.Id, TargetPixKey = "missing", Amount = 10, IdempotencyKey = "idem" };
        _redis.Setup(r => r.GetIdempotentResponseAsync<ResponsePixJson>(request.IdempotencyKey)).ReturnsAsync((ResponsePixJson?)null);
        _accountRepo.Setup(r => r.GetByIdAsync(source.Id)).ReturnsAsync((AccountEntity)source);
        _accountRepo.Setup(r => r.GetByPixKeyAsync("missing")).ReturnsAsync((AccountEntity?)null);

        var useCase = Create();
        await Assert.ThrowsAsync<NotFoundException>(() => useCase.Execute(request));
    }

    [Fact]
    public async Task ShouldThrow_WhenSameAccount()
    {
        var account = new AccountBuilder().Build();
        var request = new RequestPixJson { SourceAccountId = account.Id, TargetPixKey = account.PixKey, Amount = 10, IdempotencyKey = "idem" };
        _redis.Setup(r => r.GetIdempotentResponseAsync<ResponsePixJson>(request.IdempotencyKey)).ReturnsAsync((ResponsePixJson?)null);
        _accountRepo.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync((AccountEntity)account);
        _accountRepo.Setup(r => r.GetByPixKeyAsync(account.PixKey)).ReturnsAsync((AccountEntity)account);

        var useCase = Create();
        await Assert.ThrowsAsync<ErrorOnValidationException>(() => useCase.Execute(request));
    }

    [Fact]
    public async Task ShouldThrow_WhenAmountInvalid()
    {
        var source = new AccountBuilder().Build();
        var target = new AccountBuilder().WithPixKey("t", PixKeyType.CPF).Build();
        var request = new RequestPixJson { SourceAccountId = source.Id, TargetPixKey = "t", Amount = 0, IdempotencyKey = "idem" };
        _redis.Setup(r => r.GetIdempotentResponseAsync<ResponsePixJson>(request.IdempotencyKey)).ReturnsAsync((ResponsePixJson?)null);
        _accountRepo.Setup(r => r.GetByIdAsync(source.Id)).ReturnsAsync((AccountEntity)source);
        _accountRepo.Setup(r => r.GetByPixKeyAsync("t")).ReturnsAsync((AccountEntity)target);

        var useCase = Create();
        await Assert.ThrowsAsync<ErrorOnValidationException>(() => useCase.Execute(request));
    }

    [Fact]
    public async Task ShouldThrow_WhenLockNotAcquired()
    {
        var source = new AccountBuilder().Build();
        var target = new AccountBuilder().WithPixKey("t", PixKeyType.CPF).Build();
        var request = new RequestPixJson { SourceAccountId = source.Id, TargetPixKey = "t", Amount = 10, IdempotencyKey = "idem" };
        _redis.Setup(r => r.GetIdempotentResponseAsync<ResponsePixJson>(request.IdempotencyKey)).ReturnsAsync((ResponsePixJson?)null);
        _accountRepo.Setup(r => r.GetByIdAsync(source.Id)).ReturnsAsync((AccountEntity)source);
        _accountRepo.Setup(r => r.GetByPixKeyAsync("t")).ReturnsAsync((AccountEntity)target);
        _redis.Setup(r => r.AcquireLockAsync(It.IsAny<string>(), It.IsAny<decimal>())).ReturnsAsync(false);

        var useCase = Create();
        await Assert.ThrowsAsync<DomainException>(() => useCase.Execute(request));
    }
}