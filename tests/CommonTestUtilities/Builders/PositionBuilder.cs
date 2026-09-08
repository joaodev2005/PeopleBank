using PeopleBank.Domain.Entities;

namespace PeopleBank.CommonTestUtilities.Builders;

public class PositionBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _companyId = Guid.NewGuid();
    private string _title = "Developer";

    public PositionBuilder WithId(Guid id) { _id = id; return this; }
    public PositionBuilder WithCompanyId(Guid companyId) { _companyId = companyId; return this; }
    public PositionBuilder WithTitle(string title) { _title = title; return this; }

    public Position Build()
    {
        var position = new Position(_title, _companyId);
        var idProp = typeof(Position).GetProperty(nameof(Position.Id));
        idProp!.SetValue(position, _id);
        return position;
    }
}