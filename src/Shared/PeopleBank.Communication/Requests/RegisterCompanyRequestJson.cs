namespace PeopleBank.Communication.Requests;

public class RegisterCompanyRequestJson
{
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
}
