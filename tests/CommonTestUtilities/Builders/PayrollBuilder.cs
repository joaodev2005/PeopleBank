using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;

namespace PeopleBank.CommonTestUtilities.Builders;

public class PayrollBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _companyId = Guid.NewGuid();
    private int _month = DateTime.UtcNow.Month;
    private int _year = DateTime.UtcNow.Year;
    private PayrollStatus _status = PayrollStatus.Pending;

    public PayrollBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PayrollBuilder WithCompanyId(Guid companyId)
    {
        _companyId = companyId;
        return this;
    }

    public PayrollBuilder WithPeriod(int month, int year)
    {
        _month = month;
        _year = year;
        return this;
    }

    public PayrollBuilder WithStatus(PayrollStatus status)
    {
        _status = status;
        return this;
    }

    public Payroll Build()
    {
        var payroll = new Payroll(_companyId, _month, _year);
        var idProp = typeof(Payroll).GetProperty(nameof(Payroll.Id));
        idProp!.SetValue(payroll, _id);

        var statusProp = typeof(Payroll).GetProperty(nameof(Payroll.Status));
        statusProp!.SetValue(payroll, _status);

        if (_status == PayrollStatus.Processed)
        {
            var processedAtProp = typeof(Payroll).GetProperty(nameof(Payroll.ProcessedAt));
            processedAtProp!.SetValue(payroll, DateTime.UtcNow);
        }

        return payroll;
    }
}