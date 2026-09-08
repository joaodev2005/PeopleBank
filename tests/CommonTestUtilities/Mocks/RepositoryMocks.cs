using Moq;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;

namespace CommonTestUtilities.Mocks;

public static class RepositoryMocks
{
    public static Mock<ICompanyRepository> CreateCompanyRepository()
    {
        var mock = new Mock<ICompanyRepository>();
        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Company?)null);
        mock.Setup(r => r.GetByCnpjAsync(It.IsAny<string>())).ReturnsAsync((Company?)null);
        mock.Setup(r => r.AddAsync(It.IsAny<Company>())).Returns(Task.CompletedTask);
        mock.Setup(r => r.Update(It.IsAny<Company>()));
        return mock;
    }

    public static Mock<IEmployeeRepository> CreateEmployeeRepository()
    {
        var mock = new Mock<IEmployeeRepository>();
        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Employee?)null);
        mock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        mock.Setup(r => r.GetByCompanyAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Employee>());
        mock.Setup(r => r.AddAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);
        mock.Setup(r => r.Update(It.IsAny<Employee>()));
        return mock;
    }

    public static Mock<IAccountRepository> CreateAccountRepository()
    {
        var mock = new Mock<IAccountRepository>();
        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Account?)null);
        mock.Setup(r => r.GetByEmployeeIdAsync(It.IsAny<Guid>())).ReturnsAsync((Account?)null);
        mock.Setup(r => r.GetByPixKeyAsync(It.IsAny<string>())).ReturnsAsync((Account?)null);
        mock.Setup(r => r.AddAsync(It.IsAny<Account>())).Returns(Task.CompletedTask);
        mock.Setup(r => r.Update(It.IsAny<Account>()));
        return mock;
    }

    public static Mock<ITransactionRepository> CreateTransactionRepository()
    {
        var mock = new Mock<ITransactionRepository>();
        mock.Setup(r => r.AddAsync(It.IsAny<Transaction>())).Returns(Task.CompletedTask);
        mock.Setup(r => r.GetByIdempotencyKeyAsync(It.IsAny<string>())).ReturnsAsync((Transaction?)null);
        mock.Setup(r => r.GetByAccountAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Transaction>());
        return mock;
    }

    public static Mock<ITimeEntryRepository> CreateTimeEntryRepository()
    {
        var mock = new Mock<ITimeEntryRepository>();
        mock.Setup(r => r.AddAsync(It.IsAny<TimeEntry>())).Returns(Task.CompletedTask);
        mock.Setup(r => r.GetLastEntryAsync(It.IsAny<Guid>())).ReturnsAsync((TimeEntry?)null);
        mock.Setup(r => r.GetByEmployeeAndMonthAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<TimeEntry>());
        return mock;
    }

    public static Mock<IBenefitDefinitionRepository> CreateBenefitDefinitionRepository()
    {
        var mock = new Mock<IBenefitDefinitionRepository>();
        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((BenefitDefinition?)null);
        mock.Setup(r => r.GetByCompanyAsync(It.IsAny<Guid>())).ReturnsAsync(new List<BenefitDefinition>());
        mock.Setup(r => r.AddAsync(It.IsAny<BenefitDefinition>())).Returns(Task.CompletedTask);
        mock.Setup(r => r.Update(It.IsAny<BenefitDefinition>()));
        return mock;
    }
}