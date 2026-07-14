namespace PeopleBank.Communication.Requests;

public class CreatePositionRequestJson
{
    public string Title { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
}