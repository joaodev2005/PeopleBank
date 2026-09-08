using PeopleBank.Domain.Entities;

namespace PeopleBank.CommonTestUtilities.Builders;

public class DepartmentBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _companyId = Guid.NewGuid();
    private string _name = "Engineering";

    public DepartmentBuilder WithId(Guid id) { _id = id; return this; }
    public DepartmentBuilder WithCompanyId(Guid companyId) { _companyId = companyId; return this; }
    public DepartmentBuilder WithName(string name) { _name = name; return this; }

    public Department Build()
    {
        var dept = new Department(_name, _companyId);
        var idProp = typeof(Department).GetProperty(nameof(Department.Id));
        idProp!.SetValue(dept, _id);
        return dept;
    }
}