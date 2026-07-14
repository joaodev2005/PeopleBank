using Mapster;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Events;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Application.UseCases.Employee;

public class AddEmployeeUseCase : IAddEmployeeUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IUnitOfWork _unitOfWork;

    public AddEmployeeUseCase(
        IEmployeeRepository employeeRepository,
        ICompanyRepository companyRepository,
        IKafkaProducer kafkaProducer,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _companyRepository = companyRepository;
        _kafkaProducer = kafkaProducer;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseEmployeeJson> Execute(AddEmployeeRequestJson request)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company is null)
            throw new NotFoundException(ResourceMessagesException.COMPANY_NOT_FOUND);

        var existingCpf = await _employeeRepository.GetByCpfAsync(request.Cpf);
        if (existingCpf is not null)
            throw new ErrorOnValidationException(
                new List<string> { ResourceMessagesException.CPF_ALREADY_EXISTS });

        var employee = new Domain.Entities.Employee(
            request.Name,
            request.Cpf,
            request.Email,
            request.Salary,
            request.PixKeyType,
            request.PixKey,
            request.CompanyId,
            request.DepartmentId,
            request.PositionId
        );

        await _employeeRepository.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        var eventMessage = new EmployeeCreatedEvent
        {
            EmployeeId = employee.Id,
            Name = employee.Name,
            PixKey = employee.PixKey,
            PixKeyType = employee.PixKeyType.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        await _kafkaProducer.PublishAsync("employee-created", employee.Id.ToString(), eventMessage);

        return employee.Adapt<ResponseEmployeeJson>();
    }
}