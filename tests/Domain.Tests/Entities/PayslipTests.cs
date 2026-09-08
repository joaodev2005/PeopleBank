using FluentAssertions;
using PeopleBank.Domain.Entities;

namespace PeopleBank.Domain.Tests.Entities;

public class PayslipTests
{
    [Fact]
    public void Constructor_ShouldCalculateNetSalaryCorrectly()
    {
        var payrollId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        decimal baseSalary = 5000m;
        int overtimeHours = 10;
        decimal overtimeAmount = 500m;
        decimal discounts = 700m;

        var payslip = new Payslip(payrollId, employeeId, baseSalary, overtimeHours, overtimeAmount, discounts);

        payslip.NetSalary.Should().Be(4800m);
        payslip.BaseSalary.Should().Be(baseSalary);
        payslip.OvertimeHours.Should().Be(overtimeHours);
        payslip.OvertimeAmount.Should().Be(overtimeAmount);
        payslip.Discounts.Should().Be(discounts);
        payslip.PayrollId.Should().Be(payrollId);
        payslip.EmployeeId.Should().Be(employeeId);
        payslip.PaidAt.Should().BeNull();
    }

    [Fact]
    public void MarkAsPaid_ShouldSetPaidAt()
    {
        var payslip = new Payslip(Guid.NewGuid(), Guid.NewGuid(), 5000m, 0, 0, 500m);

        payslip.MarkAsPaid();

        payslip.PaidAt.Should().NotBeNull();
        payslip.PaidAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}