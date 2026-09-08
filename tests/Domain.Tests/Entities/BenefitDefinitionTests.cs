using FluentAssertions;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Tests.Entities;

public class BenefitDefinitionTests
{
    [Fact]
    public void Constructor_ValidData_ShouldCreate()
    {
        var companyId = Guid.NewGuid();

        var benefit = new BenefitDefinition(companyId, "VR", BenefitCategory.Meal, 800m);

        benefit.Id.Should().NotBe(Guid.Empty);
        benefit.CompanyId.Should().Be(companyId);
        benefit.Name.Should().Be("VR");
        benefit.Category.Should().Be(BenefitCategory.Meal);
        benefit.MonthlyAmount.Should().Be(800m);
        benefit.Active.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Constructor_InvalidAmount_ShouldThrow(decimal amount)
    {
        Action act = () => new BenefitDefinition(Guid.NewGuid(), "VR", BenefitCategory.Meal, amount);

        act.Should().Throw<DomainException>();
    }
}