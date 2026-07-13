namespace PeopleBank.Communication.Responses;

public class ResponseStatementJson
{
    public Guid AccountId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal AvailableBalance { get; set; }
    public List<TransactionItemJson> Transactions { get; set; } = new();
}

public class TransactionItemJson
{
    public Guid TransactionId { get; set; }
    public string Type { get; set; } = "CREDIT";
    public decimal Amount { get; set; }
    public string Counterparty { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}