using PeopleBank.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class AddPixJsonBuilder
{
    public static RequestPixJson Build() => new()
    {
        SourceAccountId = Guid.NewGuid(),
        TargetPixKey = "target-key",
        Amount = 100,
        IdempotencyKey = "idem-123"
    };
}
