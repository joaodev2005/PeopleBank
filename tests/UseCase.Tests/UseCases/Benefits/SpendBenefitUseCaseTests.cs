using FluentAssertions;
using Moq;
using PeopleBank.Application.UseCases.Benefits.SpendBenefits;
using PeopleBank.CommonTestUtilities.Builders;
using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception.ExceptionBase;

namespace UseCase.Tests.UseCases.Benefits;

public class SpendBenefitUseCaseTests
{
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly Mock<ITransactionRepository> _transactionRepo = new();
    private readonly Mock<IBenefitWalletRepository> _walletRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private SpendBenefitUseCase CreateUseCase() => new(
        _accountRepo.Object,
        _transactionRepo.Object,
        _walletRepo.Object,
        _unitOfWork.Object);

    [Fact]
    public async Task ShouldSpendBenefitSuccessfully_WhenValidRequest()
    {
        // Arrange
        var account = new AccountBuilder().WithBalance(1000).Build();
        var wallet = new BenefitWalletBuilder()
            .WithAccountId(account.Id)
            .WithBalance(500)
            .Build();

        var request = new SpendBenefitRequestJson
        {
            AccountId = account.Id,
            BenefitCategory = BenefitCategory.Meal,
            Amount = 100,
            EstablishmentCategory = EstablishmentCategory.Restaurant
        };

        _accountRepo.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);
        _walletRepo.Setup(r => r.GetByAccountAndCategoryAsync(account.Id, request.BenefitCategory)).ReturnsAsync(wallet);
        _transactionRepo.Setup(r => r.GetByIdempotencyKeyAsync(It.IsAny<string>())).ReturnsAsync((Transaction?)null);
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var useCase = CreateUseCase();

        // Act
        var result = await useCase.Execute(request);

        // Assert
        result.Should().NotBeNull();
        result.TransactionId.Should().NotBeEmpty();
        result.NewBalance.Should().Be(400);
        result.BenefitCategory.Should().Be(BenefitCategory.Meal.ToString());

        _walletRepo.Verify(r => r.Update(It.Is<BenefitWallet>(w => w.Balance == 400)), Times.Once);
        _transactionRepo.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowInsufficientBalance_WhenSaldoInsuficiente()
    {
        var account = new AccountBuilder().Build();
        var wallet = new BenefitWalletBuilder().WithAccountId(account.Id).WithBalance(50).Build();

        var request = new SpendBenefitRequestJson
        {
            AccountId = account.Id,
            BenefitCategory = BenefitCategory.Meal,
            Amount = 100,
            EstablishmentCategory = EstablishmentCategory.Restaurant
        };

        _accountRepo.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);
        _walletRepo.Setup(r => r.GetByAccountAndCategoryAsync(account.Id, request.BenefitCategory)).ReturnsAsync(wallet);

        var useCase = CreateUseCase();

        await Assert.ThrowsAsync<DomainException>(() => useCase.Execute(request));
    }

    [Fact]
    public async Task ShouldThrowInvalidEstablishment_WhenCategoriaIncompativel()
    {
        var account = new AccountBuilder().Build();
        var wallet = new BenefitWalletBuilder().WithAccountId(account.Id).WithBalance(200).Build();

        var request = new SpendBenefitRequestJson
        {
            AccountId = account.Id,
            BenefitCategory = BenefitCategory.Meal,
            Amount = 50,
            EstablishmentCategory = EstablishmentCategory.Pharmacy // not allowed for Meal
        };

        _accountRepo.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);
        _walletRepo.Setup(r => r.GetByAccountAndCategoryAsync(account.Id, request.BenefitCategory)).ReturnsAsync(wallet);

        var useCase = CreateUseCase();

        await Assert.ThrowsAsync<DomainException>(() => useCase.Execute(request));
    }

    [Fact]
    public async Task ShouldThrowAccountNotFound_WhenContaInexistente()
    {
        var request = new SpendBenefitRequestJson
        {
            AccountId = Guid.NewGuid(),
            BenefitCategory = BenefitCategory.Meal,
            Amount = 50,
            EstablishmentCategory = EstablishmentCategory.Restaurant
        };

        _accountRepo.Setup(r => r.GetByIdAsync(request.AccountId)).ReturnsAsync((PeopleBank.Domain.Entities.Account?)null);

        var useCase = CreateUseCase();

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.Execute(request));
    }
}