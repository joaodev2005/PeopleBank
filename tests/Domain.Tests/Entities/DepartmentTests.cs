using FluentAssertions;
using PeopleBank.Domain.Entities;

namespace PeopleBank.Domain.Tests.Entities;

public class DepartmentTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var companyId = Guid.NewGuid();

        var department = new Department("TI", companyId);

        department.Id.Should().NotBe(Guid.Empty);
        department.Name.Should().Be("TI");
        department.CompanyId.Should().Be(companyId);
    }
}