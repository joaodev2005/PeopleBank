namespace PeopleBank.Communication.Responses;

public class ResponsePayrollJson
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
