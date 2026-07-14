namespace PeopleBank.Communication.Responses;

public class ResponsePositionJson
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
}