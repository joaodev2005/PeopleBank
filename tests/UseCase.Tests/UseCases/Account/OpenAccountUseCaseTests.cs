using FluentAssertions;
using Moq;
using PeopleBank.Application.UseCases.Account.OpenAccount;
using PeopleBank.CommonTestUtilities.Builders;
using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception.ExceptionBase;
using AccountEntity = PeopleBank.Domain.Entities.Account;
using EmployeeEntity = PeopleBank.Domain.Entities.Employee;

namespace PeopleBank.UseCase.Tests.UseCases.Account;

public class OpenAccountUseCaseTests
{
    private readonly Mock<IAccountRepository> _accRepo = new();
    private readonly Mock<IEmployeeRepository> _empRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private OpenAccountUseCase Create() => new(_accRepo.Object, _empRepo.Object, _uow.Object);

    [Fact]
    public async Task ShouldOpenSuccessfully_WhenValid()
    {
        var employee = new EmployeeBuilder().Build();
        var request = new OpenAccountRequestJson
        {
            EmployeeId = employee.Id,
            PixKey = "new-pix",
            PixKeyType = "CPF"
        };

        _empRepo.Setup(r => r.GetByIdAsync(request.EmployeeId)).ReturnsAsync(employee);
        _accRepo.Setup(r => r.GetByEmployeeIdAsync(request.EmployeeId)).ReturnsAsync((AccountEntity?)null);
        _accRepo.Setup(r => r.GetByPixKeyAsync(request.PixKey)).ReturnsAsync((AccountEntity?)null);
        _accRepo.Setup(r => r.AddAsync(It.IsAny<AccountEntity>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var useCase = Create();
        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.PixKey.Should().Be(request.PixKey);
    }

    [Fact]
    public async Task ShouldThrow_WhenEmployeeNotFound()
    {
        var request = new OpenAccountRequestJson { EmployeeId = Guid.NewGuid(), PixKey = "k", PixKeyType = "CPF" };
        _empRepo.Setup(r => r.GetByIdAsync(request.EmployeeId)).ReturnsAsync((EmployeeEntity?)null);

        var useCase = Create();
        await Assert.ThrowsAsync<NotFoundException>(() => useCase.Execute(request));
    }

    [Fact]
    public async Task ShouldThrow_WhenAccountAlreadyExists()
    {
        var employee = new EmployeeBuilder().Build();
        var existingAcc = new AccountBuilder().WithEmployeeId(employee.Id).Build() as AccountEntity;
        var request = new OpenAccountRequestJson { EmployeeId = employee.Id, PixKey = "k", PixKeyType = "CPF" };

        _empRepo.Setup(r => r.GetByIdAsync(employee.Id)).ReturnsAsync(employee);
        _accRepo.Setup(r => r.GetByEmployeeIdAsync(employee.Id)).ReturnsAsync(existingAcc);

        var useCase = Create();
        await Assert.ThrowsAsync<ErrorOnValidationException>(() => useCase.Execute(request));
    }

    [Fact]
    public async Task ShouldThrow_WhenPixKeyDuplicate()
    {
        var employee = new EmployeeBuilder().Build();
        var otherAcc = new AccountBuilder().WithPixKey("dup", PixKeyType.CPF).Build();
        var request = new OpenAccountRequestJson { EmployeeId = employee.Id, PixKey = "dup", PixKeyType = "CPF" };

        _empRepo.Setup(r => r.GetByIdAsync(employee.Id)).ReturnsAsync(employee);
        _accRepo.Setup(r => r.GetByEmployeeIdAsync(employee.Id)).ReturnsAsync((AccountEntity?)null);
        _accRepo.Setup(r => r.GetByPixKeyAsync("dup")).ReturnsAsync(otherAcc);

        var useCase = Create();
        await Assert.ThrowsAsync<ErrorOnValidationException>(() => useCase.Execute(request));
    }

    [Fact]
    public async Task ShouldThrow_WhenInvalidPixKeyType()
    {
        var employee = new EmployeeBuilder().Build();
        var request = new OpenAccountRequestJson { EmployeeId = employee.Id, PixKey = "k", PixKeyType = "Invalid" };

        _empRepo.Setup(r => r.GetByIdAsync(employee.Id)).ReturnsAsync(employee);
        _accRepo.Setup(r => r.GetByEmployeeIdAsync(employee.Id)).ReturnsAsync((AccountEntity?)null);
        _accRepo.Setup(r => r.GetByPixKeyAsync("k")).ReturnsAsync((AccountEntity?)null);

        var useCase = Create();
        await Assert.ThrowsAsync<ErrorOnValidationException>(() => useCase.Execute(request));
    }
}