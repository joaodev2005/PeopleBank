using Mapster;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Application.UseCases.Department;

public class CreateDepartmentUseCase : ICreateDepartmentUseCase
{
    private readonly IDepartmentRepository _repository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentUseCase(
        IDepartmentRepository repository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDepartmentJson> Execute(CreateDepartmentRequestJson request)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company is null)
            throw new NotFoundException(ResourceMessagesException.COMPANY_NOT_FOUND);

        var department = new Domain.Entities.Department(request.Name, request.CompanyId);
        await _repository.AddAsync(department);
        await _unitOfWork.SaveChangesAsync();

        return department.Adapt<ResponseDepartmentJson>();
    }
}