using Mapster;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Application.UseCases.Benefits;

public class DefineBenefitUseCase : IDefineBenefitUseCase
{
    private readonly IBenefitDefinitionRepository _benefitDefinitionRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DefineBenefitUseCase(
        IBenefitDefinitionRepository benefitDefinitionRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _benefitDefinitionRepository = benefitDefinitionRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseBenefitJson> Execute(DefineBenefitRequestJson request)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company is null)
            throw new NotFoundException(ResourceMessagesException.COMPANY_NOT_FOUND);

        if (request.MonthlyAmount <= 0)
            throw new ErrorOnValidationException(
                new List<string> { "Monthly amount must be greater than zero." });

        var benefitDefinition = new BenefitDefinition(
            request.CompanyId,
            request.Name,
            request.Category,
            request.MonthlyAmount
        );

        await _benefitDefinitionRepository.AddAsync(benefitDefinition);
        await _unitOfWork.SaveChangesAsync();

        return benefitDefinition.Adapt<ResponseBenefitJson>();
    }
}