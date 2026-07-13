namespace PeopleBank.Domain.Entities;

public class Payslip
{
    public Guid Id { get; private set; }
    public Guid PayrollId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public decimal BaseSalary { get; private set; }
    public int OvertimeHours { get; private set; }
    public decimal OvertimeAmount { get; private set; }
    public decimal Discounts { get; private set; }
    public decimal NetSalary { get; private set; }
    public DateTime? PaidAt { get; private set; }

    public Payroll Payroll { get; private set; }
    public Employee Employee { get; private set; }

    private Payslip() { }

    public Payslip(
        Guid payrollId,
        Guid employeeId,
        decimal baseSalary,
        int overtimeHours,
        decimal overtimeAmount,
        decimal discounts)
    {
        Id = Guid.NewGuid();
        PayrollId = payrollId;
        EmployeeId = employeeId;
        BaseSalary = baseSalary;
        OvertimeHours = overtimeHours;
        OvertimeAmount = overtimeAmount;
        Discounts = discounts;
        NetSalary = baseSalary + overtimeAmount - discounts;
    }

    public void MarkAsPaid()
    {
        PaidAt = DateTime.UtcNow;
    }
}