using CommonTestUtilities.Mocks;
using FluentAssertions;
using Moq;
using PeopleBank.Application.UseCases.Benefits.DefineBenefits;
using PeopleBank.CommonTestUtilities.Builders;
using PeopleBank.CommonTestUtilities.Requests;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception.ExceptionBase;

namespace UseCase.Tests.UseCases.Benefits;

public class DefineBenefitUseCaseTests
{
    private readonly Mock<IBenefitDefinitionRepository> _benefitRepoMock;
    private readonly Mock<ICompanyRepository> _companyRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly DefineBenefitUseCase _useCase;

    public DefineBenefitUseCaseTests()
    {
        _benefitRepoMock = RepositoryMocks.CreateBenefitDefinitionRepository();
        _companyRepoMock = RepositoryMocks.CreateCompanyRepository();
        _uowMock = new Mock<IUnitOfWork>();
        _useCase = new DefineBenefitUseCase(
            _benefitRepoMock.Object,
            _companyRepoMock.Object,
            _uowMock.Object);
    }

    [Fact]
    public async Task Execute_Valid_ShouldCreateBenefit()
    {
        var company = new CompanyBuilder().Build();
        _companyRepoMock.Setup(r => r.GetByIdAsync(company.Id))
                        .ReturnsAsync(company);
        var request = DefineBenefitRequestBuilder.Build(company.Id);

        var result = await _useCase.Execute(request);

        result.Should().NotBeNull();
        _benefitRepoMock.Verify(r => r.AddAsync(It.IsAny<BenefitDefinition>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Execute_CompanyNotFound_ShouldThrow()
    {
        _companyRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                        .ReturnsAsync((PeopleBank.Domain.Entities.Company?)null);
        var request = DefineBenefitRequestBuilder.Build();

        Func<Task> act = () => _useCase.Execute(request);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}