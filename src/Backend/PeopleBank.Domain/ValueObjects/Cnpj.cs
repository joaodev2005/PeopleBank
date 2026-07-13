using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.ValueObjects;

public class Cnpj
{
    public string Value { get; private set; }

    private Cnpj() { }

    public Cnpj(string cnpj)
    {
        if (!IsValid(cnpj))
            throw new ErrorOnValidationException(new List<string> { ResourceMessagesException.INVALID_CNPJ });

        Value = Sanitize(cnpj);
    }

    public static bool IsValid(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        cnpj = Sanitize(cnpj);

        if (cnpj.Length != 14 || cnpj.All(c => c == cnpj[0]))
            return false;

        int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var baseCnpj = cnpj[..12];
        var sum = 0;
        for (int i = 0; i < 12; i++)
            sum += int.Parse(baseCnpj[i].ToString()) * multiplier1[i];

        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (digit1 != int.Parse(cnpj[12].ToString()))
            return false;

        baseCnpj += digit1;
        sum = 0;
        for (int i = 0; i < 13; i++)
            sum += int.Parse(baseCnpj[i].ToString()) * multiplier2[i];

        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return digit2 == int.Parse(cnpj[13].ToString());
    }

    private static string Sanitize(string cnpj) =>
        cnpj?.Replace(".", "").Replace("-", "").Replace("/", "").Trim() ?? string.Empty;

    public override string ToString() => Value;
    public static implicit operator string(Cnpj cnpj) => cnpj.Value;
}