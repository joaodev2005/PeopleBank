using PeopleBank.Domain.Enums;

namespace PeopleBank.Domain.Entities;

public class Payroll
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }
    public PayrollStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    public Company Company { get; private set; }
    private readonly List<Payslip> _payslips = new();
    public IReadOnlyCollection<Payslip> Payslips => _payslips.AsReadOnly();

    private Payroll() { }

    public Payroll(Guid companyId, int month, int year)
    {
        Id = Guid.NewGuid();
        CompanyId = companyId;
        Month = month;
        Year = year;
        Status = PayrollStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    internal void AddPayslip(Payslip payslip)
    {
        _payslips.Add(payslip);
    }

    public void MarkAsProcessed()
    {
        Status = PayrollStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
    }
}