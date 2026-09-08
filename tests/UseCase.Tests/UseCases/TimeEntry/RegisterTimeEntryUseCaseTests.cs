using CommonTestUtilities.Mocks;
using FluentAssertions;
using Moq;
using PeopleBank.Application.UseCases.TimeEntry;
using PeopleBank.CommonTestUtilities.Builders;
using PeopleBank.CommonTestUtilities.Requests;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception.ExceptionBase;

namespace UseCase.Tests.UseCases.TimeEntry;

public class RegisterTimeEntryUseCaseTests
{
    private readonly Mock<ITimeEntryRepository> _timeEntryRepoMock;
    private readonly Mock<IEmployeeRepository> _employeeRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly RegisterTimeEntryUseCase _useCase;

    public RegisterTimeEntryUseCaseTests()
    {
        _timeEntryRepoMock = RepositoryMocks.CreateTimeEntryRepository();
        _employeeRepoMock = RepositoryMocks.CreateEmployeeRepository();
        _uowMock = new Mock<IUnitOfWork>();
        _useCase = new RegisterTimeEntryUseCase(
            _timeEntryRepoMock.Object,
            _employeeRepoMock.Object,
            _uowMock.Object);
    }

    [Fact]
    public async Task Execute_Valid_ShouldRegister()
    {
        var employee = new EmployeeBuilder().Build();
        _employeeRepoMock.Setup(r => r.GetByIdAsync(employee.Id))
                        .ReturnsAsync(employee);
        _timeEntryRepoMock.Setup(r => r.GetLastEntryAsync(employee.Id))
                          .ReturnsAsync((PeopleBank.Domain.Entities.TimeEntry?)null);

        var request = RegisterTimeEntryRequestBuilder.Build(employee.Id, TimeEntryType.ClockIn);

        var result = await _useCase.Execute(request);

        result.Should().NotBeNull();
        _timeEntryRepoMock.Verify(r => r.AddAsync(It.IsAny<PeopleBank.Domain.Entities.TimeEntry>()), Times.Once);
    }

    [Fact]
    public async Task Execute_ConsecutiveSameType_ShouldThrow()
    {
        var employee = new EmployeeBuilder().Build();
        var lastEntry = new PeopleBank.Domain.Entities.TimeEntry(employee.Id, DateTime.UtcNow, TimeEntryType.ClockIn);
        _employeeRepoMock.Setup(r => r.GetByIdAsync(employee.Id))
                        .ReturnsAsync(employee);
        _timeEntryRepoMock.Setup(r => r.GetLastEntryAsync(employee.Id))
                          .ReturnsAsync(lastEntry);

        var request = RegisterTimeEntryRequestBuilder.Build(employee.Id, TimeEntryType.ClockIn);

        Func<Task> act = () => _useCase.Execute(request);

        await act.Should().ThrowAsync<ErrorOnValidationException>();
    }
}