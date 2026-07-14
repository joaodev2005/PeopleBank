namespace PeopleBank.Communication.Responses;

public class ResponseDepartmentJson
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
}