using FluentAssertions;
using PeopleBank.Domain.ValueObjects;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.Tests.ValueObjects;

public class CpfTests
{
    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52035968836")]
    [InlineData("39053344705")]
    public void Constructor_ValidCpf_ShouldCreate(string cpf)
    {
        var cpfObj = new Cpf(cpf);

        cpfObj.Value.Should().Be(cpf.Replace(".", "").Replace("-", ""));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("11111111111")]
    [InlineData("")]
    [InlineData(null)]
    public void Constructor_InvalidCpf_ShouldThrowException(string? cpf)
    {
        Action act = () => new Cpf(cpf!);

        act.Should().Throw<ErrorOnValidationException>();
    }
}