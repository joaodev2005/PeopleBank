using CommonTestUtilities.Mocks;
using FluentAssertions;
using Moq;
using PeopleBank.Application.UseCases.Employee;
using PeopleBank.CommonTestUtilities.Builders;
using PeopleBank.CommonTestUtilities.Requests;
using PeopleBank.Domain.Events;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception.ExceptionBase;

namespace UseCase.Tests.UseCases.Account.Employee;

public class AddEmployeeUseCaseTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepoMock;
    private readonly Mock<ICompanyRepository> _companyRepoMock;
    private readonly Mock<IKafkaProducer> _kafkaMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly AddEmployeeUseCase _useCase;

    public AddEmployeeUseCaseTests()
    {
        _employeeRepoMock = RepositoryMocks.CreateEmployeeRepository();
        _companyRepoMock = RepositoryMocks.CreateCompanyRepository();
        _kafkaMock = ServiceMocks.CreateKafkaProducer();
        _uowMock = new Mock<IUnitOfWork>();
        _useCase = new AddEmployeeUseCase(
            _employeeRepoMock.Object,
            _companyRepoMock.Object,
            _kafkaMock.Object,
            _uowMock.Object);
    }

    [Fact]
    public async Task Execute_ValidRequest_ShouldAddAndPublishEvent()
    {
        var request = AddEmployeeRequestBuilder.Build();
        var company = new CompanyBuilder().Build();
        _companyRepoMock.Setup(r => r.GetByIdAsync(request.CompanyId))
                        .ReturnsAsync(company);
        _employeeRepoMock.Setup(r => r.GetByCpfAsync(request.Cpf))
                         .ReturnsAsync((PeopleBank.Domain.Entities.Employee?)null);

        var result = await _useCase.Execute(request);

        result.Should().NotBeNull();
        _employeeRepoMock.Verify(r => r.AddAsync(It.IsAny<PeopleBank.Domain.Entities.Employee>()), Times.Once);
        _kafkaMock.Verify(k => k.PublishAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<EmployeeCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task Execute_CompanyNotFound_ShouldThrow()
    {
        var request = AddEmployeeRequestBuilder.Build();
        _companyRepoMock.Setup(r => r.GetByIdAsync(request.CompanyId))
                        .ReturnsAsync((PeopleBank.Domain.Entities.Company?)null);

        Func<Task> act = () => _useCase.Execute(request);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Execute_DuplicateCpf_ShouldThrow()
    {
        var request = AddEmployeeRequestBuilder.Build();
        _companyRepoMock.Setup(r => r.GetByIdAsync(request.CompanyId))
                        .ReturnsAsync(new CompanyBuilder().Build());
        _employeeRepoMock.Setup(r => r.GetByCpfAsync(request.Cpf))
                         .ReturnsAsync(new EmployeeBuilder().Build());

        Func<Task> act = () => _useCase.Execute(request);

        await act.Should().ThrowAsync<ErrorOnValidationException>();
    }
}