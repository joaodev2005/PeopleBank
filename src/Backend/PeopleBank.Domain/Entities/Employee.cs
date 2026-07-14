// PeopleBank.Domain/Entities/Employee.cs
using System.Security.Principal;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.ValueObjects;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Entities;

public class Employee
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Cpf Cpf { get; private set; }
    public Email Email { get; private set; }
    public decimal Salary { get; private set; }
    public PixKeyType PixKeyType { get; private set; }
    public string PixKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool Active { get; private set; }

    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; }

    public Guid DepartmentId { get; private set; }
    public Department Department { get; private set; }

    public Guid PositionId { get; private set; }
    public Position Position { get; private set; }

    public Account? Account { get; private set; }

    private readonly List<TimeEntry> _timeEntries = new();
    public IReadOnlyCollection<TimeEntry> TimeEntries => _timeEntries.AsReadOnly();

    private Employee() { }

    public Employee(
        string name,
        string cpf,
        string email,
        decimal salary,
        PixKeyType pixKeyType,
        string pixKey,
        Guid companyId,
        Guid departmentId,
        Guid positionId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Cpf = new Cpf(cpf);
        Email = new Email(email);
        Salary = salary;
        PixKeyType = pixKeyType;
        PixKey = pixKey;
        CompanyId = companyId;
        DepartmentId = departmentId;
        PositionId = positionId;
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public void UpdateSalary(decimal newSalary)
    {
        if (newSalary <= 0)
            throw new DomainException("Salary must be greater than zero.");
        Salary = newSalary;
    }

    public void ChangeDepartment(Guid newDepartmentId)
    {
        DepartmentId = newDepartmentId;
    }

    public void ChangePosition(Guid newPositionId)
    {
        PositionId = newPositionId;
    }

    public void Deactivate()
    {
        Active = false;
    }

    public void Activate()
    {
        Active = true;
    }

    public void AssignAccount(Account account)
    {
        Account = account;
    }
}