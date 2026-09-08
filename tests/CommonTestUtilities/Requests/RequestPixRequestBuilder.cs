using PeopleBank.Communication.Requests;

namespace PeopleBank.CommonTestUtilities.Requests;

public class RequestPixRequestBuilder
{
    public static RequestPixJson Build(
        Guid? sourceAccountId = null,
        string? targetPixKey = null,
        decimal amount = 100,
        string? idempotencyKey = null)
    {
        return new RequestPixJson
        {
            SourceAccountId = sourceAccountId ?? Guid.NewGuid(),
            TargetPixKey = targetPixKey ?? "target@example.com",
            Amount = amount,
            IdempotencyKey = idempotencyKey ?? Guid.NewGuid().ToString()
        };
    }
}