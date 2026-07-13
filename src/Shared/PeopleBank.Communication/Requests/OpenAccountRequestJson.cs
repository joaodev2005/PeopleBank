namespace PeopleBank.Communication.Requests;

public class OpenAccountRequestJson
{
    public Guid EmployeeId { get; set; }
    public string PixKey { get; set; } = string.Empty;
    public string PixKeyType { get; set; } = "Email"; 
}