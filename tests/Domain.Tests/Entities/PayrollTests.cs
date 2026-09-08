using FluentAssertions;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;

namespace PeopleBank.Domain.Tests.Entities;

public class PayrollTests
{
    private Payroll CreatePendingPayroll()
    {
        return new Payroll(Guid.NewGuid(), 8, 2026);
    }

    [Fact]
    public void AddPayslip_ShouldAddToCollection()
    {
        var payroll = CreatePendingPayroll();
        var payslip = new Payslip(payroll.Id, Guid.NewGuid(), 5000m, 0, 0m, 500m);

        payroll.AddPayslip(payslip);

        payroll.Payslips.Should().ContainSingle();
        payroll.Payslips.First().Should().Be(payslip);
    }

    [Fact]
    public void MarkAsProcessed_ShouldSetStatusAndDate()
    {
        var payroll = CreatePendingPayroll();

        payroll.MarkAsProcessed();

        payroll.Status.Should().Be(PayrollStatus.Processed);
        payroll.ProcessedAt.Should().NotBeNull();
    }
}