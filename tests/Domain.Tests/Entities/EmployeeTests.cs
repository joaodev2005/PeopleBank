using FluentAssertions;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Tests.Entities;

public class EmployeeTests
{
    private Employee CreateEmployee()
    {
        return new Employee(
            "João",
            "52998224725",
            "joao@teste.com",
            5000m,
            PixKeyType.Email,
            "joao@teste.com",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()
        );
    }

    [Fact]
    public void AssignAccount_ShouldSetAccountProperty()
    {
        var employee = CreateEmployee();
        var account = new Account(employee.Id, "joao@teste.com", PixKeyType.Email);

        employee.AssignAccount(account);

        employee.Account.Should().Be(account);
    }

    [Theory]
    [InlineData(10000)]
    [InlineData(5000.50)]
    public void UpdateSalary_WithValidValue_ShouldUpdate(decimal newSalary)
    {
        var employee = CreateEmployee();

        employee.UpdateSalary(newSalary);

        employee.Salary.Should().Be(newSalary);
    }

    [Fact]
    public void UpdateSalary_WithInvalidValue_ShouldThrow()
    {
        var employee = CreateEmployee();

        Action act = () => employee.UpdateSalary(0);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Deactivate_ShouldSetActiveFalse()
    {
        var employee = CreateEmployee();

        employee.Deactivate();

        employee.Active.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetActiveTrue()
    {
        var employee = CreateEmployee();
        employee.Deactivate();

        employee.Activate();

        employee.Active.Should().BeTrue();
    }
}