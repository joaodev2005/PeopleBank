//using PeopleBank.Domain.Entities;

//namespace PeopleBank.CommonTestUtilities.Builders;

//public class CompanyBuilder
//{
//    private Guid _id = Guid.NewGuid();
//    private string _name = "Test Company";
//    private string _cnpj = "12345678000195";

//    public CompanyBuilder WithId(Guid id)
//    {
//        _id = id;
//        return this;
//    }

//    public CompanyBuilder WithName(string name)
//    {
//        _name = name;
//        return this;
//    }

//    public CompanyBuilder WithCnpj(string cnpj)
//    {
//        _cnpj = cnpj;
//        return this;
//    }

//    public Company Build()
//    {
//        var company = new Company(_name, _cnpj);
//        var idProp = typeof(Company).GetProperty(nameof(Company.Id));
//        idProp!.SetValue(company, _id);
//        return company;
//    }
//}

using PeopleBank.Domain.Entities;

namespace PeopleBank.CommonTestUtilities.Builders;

public class CompanyBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Company";
    private string _cnpj = string.Empty; // agora gera dinamicamente

    public CompanyBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CompanyBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CompanyBuilder WithCnpj(string cnpj)
    {
        _cnpj = cnpj;
        return this;
    }

    public Company Build()
    {
        if (string.IsNullOrEmpty(_cnpj))
            _cnpj = GenerateValidCnpj();

        var company = new Company(_name, _cnpj);
        typeof(Company).GetProperty(nameof(Company.Id))!.SetValue(company, _id);
        return company;
    }

    private static string GenerateValidCnpj()
    {
        var random = new Random();
        var digits = new int[14];
        for (int i = 0; i < 12; i++)
            digits[i] = random.Next(0, 9);

        digits[12] = CalculateDigit(digits, new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });
        digits[13] = CalculateDigit(digits, new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });

        return string.Join("", digits);
    }

    private static int CalculateDigit(int[] cnpj, int[] multipliers)
    {
        int sum = 0;
        for (int i = 0; i < multipliers.Length; i++)
            sum += cnpj[i] * multipliers[i];

        int remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}