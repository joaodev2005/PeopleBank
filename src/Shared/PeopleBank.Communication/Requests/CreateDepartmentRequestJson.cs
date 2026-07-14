namespace PeopleBank.Communication.Requests;

public class CreateDepartmentRequestJson
{
    public string Name { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
}