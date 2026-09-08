using Mapster;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;


namespace PeopleBank.Application.UseCases.Account.OpenAccount;

public class OpenAccountUseCase : IOpenAccountUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OpenAccountUseCase(
        IAccountRepository accountRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseAccountJson> Execute(OpenAccountRequestJson request)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        if (employee is null)
            throw new NotFoundException(ResourceMessagesException.EMPLOYEE_NOT_FOUND);

        var existingAccount = await _accountRepository.GetByEmployeeIdAsync(request.EmployeeId);
        if (existingAccount is not null)
            throw new ErrorOnValidationException(
                new List<string> { ResourceMessagesException.ACCOUNT_ALREADY_EXISTS });

        var pixKeyExists = await _accountRepository.GetByPixKeyAsync(request.PixKey);
        if (pixKeyExists is not null)
            throw new ErrorOnValidationException(
                new List<string> { ResourceMessagesException.PIX_KEY_ALREADY_EXISTS });

        if (!Enum.TryParse<PixKeyType>(request.PixKeyType, ignoreCase: true, out var pixKeyType))
            throw new ErrorOnValidationException(
                new List<string> { ResourceMessagesException.INVALID_PIX_KEY_TYPE });

        var account = new Domain.Entities.Account(request.EmployeeId, request.PixKey, pixKeyType);

        employee.AssignAccount(account);

        await _accountRepository.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();

        return account.Adapt<ResponseAccountJson>();
    }
}