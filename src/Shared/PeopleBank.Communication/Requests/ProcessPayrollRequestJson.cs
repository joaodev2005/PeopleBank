namespace PeopleBank.Communication.Requests;

public class ProcessPayrollRequestJson
{
    public Guid CompanyId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}