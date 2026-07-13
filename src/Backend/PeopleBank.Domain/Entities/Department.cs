namespace PeopleBank.Domain.Entities;

public class Department
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid CompanyId { get; private set; }

    public Company Company { get; private set; }
    private readonly List<Employee> _employees = new();
    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();

    private Department() { }

    public Department(string name, Guid companyId)
    {
        Id = Guid.NewGuid();
        Name = name;
        CompanyId = companyId;
    }
}