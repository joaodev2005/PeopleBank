using PeopleBank.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class AddCompanyRequestJsonBuilder
{
    public static RegisterCompanyRequestJson Build() => new()
    {
        Name = "Acme",
        Cnpj = "12345678000195"
    };
}
