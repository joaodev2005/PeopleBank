using Mapster;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Application.UseCases.Company;

public class RegisterCompanyUseCase : IRegisterCompanyUseCase
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCompanyUseCase(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseCompanyJson> Execute(RegisterCompanyRequestJson request)
    {
        var existingCompany = await _companyRepository.GetByCnpjAsync(request.Cnpj);
        if (existingCompany is not null)
            throw new ErrorOnValidationException(
                new List<string> { ResourceMessagesException.COMPANY_ALREADY_EXISTS });

        var company = new Domain.Entities.Company(request.Name, request.Cnpj);

        await _companyRepository.AddAsync(company);
        await _unitOfWork.SaveChangesAsync();

        return company.Adapt<ResponseCompanyJson>();
    }
}