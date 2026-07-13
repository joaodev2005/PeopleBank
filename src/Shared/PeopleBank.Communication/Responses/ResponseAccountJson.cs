namespace PeopleBank.Communication.Responses;

public class ResponseAccountJson
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public decimal Balance { get; set; }
    public decimal BlockedBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public string PixKey { get; set; } = string.Empty;
    public string PixKeyType { get; set; } = string.Empty;
}