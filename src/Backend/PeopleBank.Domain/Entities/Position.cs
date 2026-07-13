namespace PeopleBank.Domain.Entities;

public class Position
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public Guid CompanyId { get; private set; }

    public Company Company { get; private set; }
    private readonly List<Employee> _employees = new();
    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();

    private Position() { }

    public Position(string title, Guid companyId)
    {
        Id = Guid.NewGuid();
        Title = title;
        CompanyId = companyId;
    }
}