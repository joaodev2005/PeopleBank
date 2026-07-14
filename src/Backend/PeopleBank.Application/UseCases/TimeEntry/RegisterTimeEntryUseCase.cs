using Mapster;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Application.UseCases.TimeEntry;

public class RegisterTimeEntryUseCase : IRegisterTimeEntryUseCase
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterTimeEntryUseCase(
        ITimeEntryRepository timeEntryRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _timeEntryRepository = timeEntryRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseTimeEntryJson> Execute(RegisterTimeEntryRequestJson request)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        if (employee is null)
            throw new NotFoundException(ResourceMessagesException.EMPLOYEE_NOT_FOUND);

        if (!Enum.TryParse<TimeEntryType>(request.Type, ignoreCase: true, out var entryType))
            throw new ErrorOnValidationException(
                new List<string> { "Invalid time entry type. Must be 'ClockIn' or 'ClockOut'." });

        var lastEntry = await _timeEntryRepository.GetLastEntryAsync(request.EmployeeId);
        if (lastEntry is not null && lastEntry.Type == entryType)
            throw new ErrorOnValidationException(
                new List<string> { "Cannot register two consecutive entries of the same type." });

        var timeEntry = new Domain.Entities.TimeEntry(request.EmployeeId, request.Timestamp, entryType);

        await _timeEntryRepository.AddAsync(timeEntry);
        await _unitOfWork.SaveChangesAsync();

        return timeEntry.Adapt<ResponseTimeEntryJson>();
    }
}