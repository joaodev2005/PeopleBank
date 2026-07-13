namespace PeopleBank.Communication.Requests;

public class RegisterTimeEntryRequestJson
{
    public Guid EmployeeId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = "ClockIn"; 
}