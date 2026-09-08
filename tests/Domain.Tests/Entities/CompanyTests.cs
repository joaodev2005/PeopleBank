using FluentAssertions;
using PeopleBank.Domain.Entities;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Tests.Entities;

public class CompanyTests
{
    [Fact]
    public void Constructor_ValidData_ShouldCreateCompany()
    {
        var company = new Company("Empresa X", "11222333000181");

        company.Name.Should().Be("Empresa X");
        company.Cnpj.Value.Should().Be("11222333000181");
        company.Active.Should().BeTrue();
    }

    [Fact]
    public void Constructor_InvalidCnpj_ShouldThrow()
    {
        Action act = () => new Company("Empresa X", "123");

        act.Should().Throw<ErrorOnValidationException>();
    }

    [Fact]
    public void UpdateName_ShouldChangeName()
    {
        var company = new Company("Old", "11222333000181");

        company.UpdateName("New");

        company.Name.Should().Be("New");
    }

    [Fact]
    public void Deactivate_ShouldSetActiveFalse()
    {
        var company = new Company("Empresa", "11222333000181");

        company.Deactivate();

        company.Active.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetActiveTrue()
    {
        var company = new Company("Empresa", "11222333000181");
        company.Deactivate();

        company.Activate();

        company.Active.Should().BeTrue();
    }
}