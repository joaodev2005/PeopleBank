using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.ValueObjects;

public class Cpf
{
    public string Value { get; private set; }

    private Cpf() { } 

    public Cpf(string cpf)
    {
        if (!IsValid(cpf))
            throw new ErrorOnValidationException(new List<string> { ResourceMessagesException.INVALID_CPF });

        Value = Sanitize(cpf);
    }

    public static bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = Sanitize(cpf);

        if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
            return false;

        int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var baseCpf = cpf[..9];
        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(baseCpf[i].ToString()) * multiplier1[i];

        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (digit1 != int.Parse(cpf[9].ToString()))
            return false;

        baseCpf += digit1;
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(baseCpf[i].ToString()) * multiplier2[i];

        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return digit2 == int.Parse(cpf[10].ToString());
    }

    private static string Sanitize(string cpf) =>
        cpf?.Replace(".", "").Replace("-", "").Trim() ?? string.Empty;

    public override string ToString() => Value;

    public static implicit operator string(Cpf cpf) => cpf.Value;
}