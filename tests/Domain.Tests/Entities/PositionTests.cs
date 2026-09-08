using FluentAssertions;
using PeopleBank.Domain.Entities;

namespace PeopleBank.Domain.Tests.Entities;

public class PositionTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var companyId = Guid.NewGuid();

        var position = new Position("Dev", companyId);

        position.Id.Should().NotBe(Guid.Empty);
        position.Title.Should().Be("Dev");
        position.CompanyId.Should().Be(companyId);
    }
}