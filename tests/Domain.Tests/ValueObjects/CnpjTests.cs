using FluentAssertions;
using PeopleBank.Domain.ValueObjects;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Tests.ValueObjects;

public class CnpjTests
{
    [Theory]
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11222333000181")]
    public void Constructor_ValidCnpj_ShouldCreate(string cnpj)
    {
        var cnpjObj = new Cnpj(cnpj);

        cnpjObj.Value.Should().Be(cnpj.Replace(".", "").Replace("/", "").Replace("-", ""));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("11111111111111")]
    [InlineData("")]
    [InlineData(null)]
    public void Constructor_InvalidCnpj_ShouldThrowException(string? cnpj)
    {
        Action act = () => new Cnpj(cnpj!);

        act.Should().Throw<ErrorOnValidationException>();
    }
}