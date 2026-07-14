using Mapster;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Events;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Application.UseCases.Payroll;

public class ProcessPayrollUseCase : IProcessPayrollUseCase
{
    private readonly IPayrollRepository _payrollRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPayrollUseCase(
        IPayrollRepository payrollRepository,
        ICompanyRepository companyRepository,
        IKafkaProducer kafkaProducer,
        IUnitOfWork unitOfWork)
    {
        _payrollRepository = payrollRepository;
        _companyRepository = companyRepository;
        _kafkaProducer = kafkaProducer;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponsePayrollJson> Execute(ProcessPayrollRequestJson request)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company is null)
            throw new NotFoundException(ResourceMessagesException.COMPANY_NOT_FOUND);

        var existingPayrolls = await _payrollRepository.GetByCompanyAndPeriodAsync(
            request.CompanyId, request.Month, request.Year);

        if (existingPayrolls.Any(p => p.Status == Domain.Enums.PayrollStatus.Pending))
            throw new ErrorOnValidationException(
                new List<string> { "A pending payroll already exists for this period." });

        var payroll = new Domain.Entities.Payroll(request.CompanyId, request.Month, request.Year);

        await _payrollRepository.AddAsync(payroll);
        await _unitOfWork.SaveChangesAsync();

        var eventMessage = new PayrollRequestedEvent
        {
            PayrollId = payroll.Id,
            CompanyId = payroll.CompanyId,
            Month = payroll.Month,
            Year = payroll.Year,
            RequestedAt = DateTime.UtcNow
        };

        await _kafkaProducer.PublishAsync("payroll-requested", payroll.Id.ToString(), eventMessage);

        return payroll.Adapt<ResponsePayrollJson>();
    }
}