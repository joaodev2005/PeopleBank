using FluentAssertions;
using Moq;
using PeopleBank.Application.UseCases.Payroll;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception.ExceptionBase;
using PeopleBank.CommonTestUtilities.Builders;
using Xunit;
using PayrollEntity = PeopleBank.Domain.Entities.Payroll;
using PayrollRequestedEventEntity = PeopleBank.Domain.Events.PayrollRequestedEvent;

namespace PeopleBank.UseCase.Tests.UseCases.Payroll;

public class ProcessPayrollUseCaseTests
{
    private readonly Mock<IPayrollRepository> _payrollRepo = new();
    private readonly Mock<ICompanyRepository> _companyRepo = new();
    private readonly Mock<IKafkaProducer> _kafka = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private ProcessPayrollUseCase Create() => new(_payrollRepo.Object, _companyRepo.Object, _kafka.Object, _uow.Object);

    [Fact]
    public async Task ShouldProcessSuccessfully_WhenValid()
    {
        var company = new CompanyBuilder().Build();
        var request = new ProcessPayrollRequestJson { CompanyId = company.Id, Month = 8, Year = 2026 };

        _companyRepo.Setup(r => r.GetByIdAsync(company.Id)).ReturnsAsync(company);
        _payrollRepo.Setup(r => r.GetByCompanyAndPeriodAsync(company.Id, request.Month, request.Year))
            .ReturnsAsync(new List<PayrollEntity>());
        _payrollRepo.Setup(r => r.AddAsync(It.IsAny<PayrollEntity>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _kafka.Setup(k => k.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PayrollRequestedEventEntity>()))
            .Returns(Task.CompletedTask);

        var useCase = Create();
        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Month.Should().Be(request.Month);
        _kafka.Verify(k => k.PublishAsync("payroll-requested", It.IsAny<string>(), It.IsAny<PayrollRequestedEventEntity>()), Times.Once);
    }

    [Fact]
    public async Task ShouldThrow_WhenCompanyNotFound()
    {
        var request = new ProcessPayrollRequestJson { CompanyId = Guid.NewGuid(), Month = 8, Year = 2026 };
        _companyRepo.Setup(r => r.GetByIdAsync(request.CompanyId)).ReturnsAsync((Company?)null);

        var useCase = Create();
        await Assert.ThrowsAsync<NotFoundException>(() => useCase.Execute(request));
    }

    [Fact]
    public async Task ShouldThrow_WhenPendingPayrollExists()
    {
        var company = new CompanyBuilder().Build();
        var existing = new PayrollBuilder().WithCompanyId(company.Id).WithPeriod(8,2026).WithStatus(PayrollStatus.Pending).Build();
        var request = new ProcessPayrollRequestJson { CompanyId = company.Id, Month = 8, Year = 2026 };

        _companyRepo.Setup(r => r.GetByIdAsync(company.Id)).ReturnsAsync(company);
        _payrollRepo.Setup(r => r.GetByCompanyAndPeriodAsync(company.Id, 8, 2026))
            .ReturnsAsync(new List<PayrollEntity>{ existing });

        var useCase = Create();
        await Assert.ThrowsAsync<ErrorOnValidationException>(() => useCase.Execute(request));
    }
}