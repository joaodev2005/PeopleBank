using CommonTestUtilities.Mocks;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Moq;
using PeopleBank.Application.UseCases.Company;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception.ExceptionBase;

namespace UseCase.Tests.UseCases.Company;

public class RegisterCompanyUseCaseTests
{
    private readonly Mock<ICompanyRepository> _companyRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly RegisterCompanyUseCase _useCase;

    public RegisterCompanyUseCaseTests()
    {
        _companyRepoMock = RepositoryMocks.CreateCompanyRepository();
        _uowMock = new Mock<IUnitOfWork>();
        _useCase = new RegisterCompanyUseCase(_companyRepoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Execute_ValidRequest_ShouldCreateAndReturnCompany()
    {
        var request = AddCompanyRequestJsonBuilder.Build();
        _companyRepoMock.Setup(r => r.GetByCnpjAsync(request.Cnpj))
                        .ReturnsAsync((PeopleBank.Domain.Entities.Company?)null);

        var result = await _useCase.Execute(request);

        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        _companyRepoMock.Verify(r => r.AddAsync(It.IsAny<PeopleBank.Domain.Entities.Company>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Execute_ExistingCnpj_ShouldThrowErrorOnValidation()
    {
        var request = AddCompanyRequestJsonBuilder.Build();
        var existing = new PeopleBank.Domain.Entities.Company(request.Name, request.Cnpj);
        _companyRepoMock.Setup(r => r.GetByCnpjAsync(request.Cnpj))
                        .ReturnsAsync(existing);

        Func<Task> act = () => _useCase.Execute(request);

        await act.Should().ThrowAsync<ErrorOnValidationException>();
        _companyRepoMock.Verify(r => r.AddAsync(It.IsAny<PeopleBank.Domain.Entities.Company>()), Times.Never);
    }
}