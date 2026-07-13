using PeopleBank.Domain.ValueObjects;

namespace PeopleBank.Domain.Entities;

public class Company
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Cnpj Cnpj { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool Active { get; private set; }

    private readonly List<Department> _departments = new();
    public IReadOnlyCollection<Department> Departments => _departments.AsReadOnly();

    private readonly List<Employee> _employees = new();
    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();

    private Company() { } 

    public Company(string name, string cnpj)
    {
        Id = Guid.NewGuid();
        Name = name;
        Cnpj = new Cnpj(cnpj); 
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void Deactivate()
    {
        Active = false;
    }

    public void Activate()
    {
        Active = true;
    }
}