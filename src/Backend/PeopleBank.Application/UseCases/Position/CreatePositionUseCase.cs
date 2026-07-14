using Mapster;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Application.UseCases.Position;

public class CreatePositionUseCase : ICreatePositionUseCase
{
    private readonly IPositionRepository _repository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePositionUseCase(
        IPositionRepository repository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponsePositionJson> Execute(CreatePositionRequestJson request)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company is null)
            throw new NotFoundException(ResourceMessagesException.COMPANY_NOT_FOUND);

        var position = new Domain.Entities.Position(request.Title, request.CompanyId);
        await _repository.AddAsync(position);
        await _unitOfWork.SaveChangesAsync();

        return position.Adapt<ResponsePositionJson>();
    }
}