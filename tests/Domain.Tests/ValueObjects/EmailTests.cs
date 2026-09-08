using FluentAssertions;
using PeopleBank.Domain.ValueObjects;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("joao@teste.com")]
    [InlineData("maria.silva@example.org")]
    public void Constructor_ValidEmail_ShouldCreate(string email)
    {
        var emailObj = new Email(email);

        emailObj.Value.Should().Be(email.Trim().ToLowerInvariant());
    }

    [Theory]
    [InlineData("joao")]
    [InlineData("joao@")]
    [InlineData("")]
    [InlineData(null)]
    public void Constructor_InvalidEmail_ShouldThrow(string? email)
    {
        Action act = () => new Email(email!);

        act.Should().Throw<ErrorOnValidationException>();
    }
}