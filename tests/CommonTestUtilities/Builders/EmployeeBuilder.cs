using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;

namespace PeopleBank.CommonTestUtilities.Builders;

public class EmployeeBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "John Doe";
    private string _cpf = "39053344705";
    private string _email = "john@example.com";
    private decimal _salary = 5000m;
    private PixKeyType _pixKeyType = PixKeyType.CPF;
    private string _pixKey = "39053344705";
    private Guid _companyId = Guid.NewGuid();
    private Guid _departmentId = Guid.NewGuid();
    private Guid _positionId = Guid.NewGuid();
    private bool _active = true;

    public EmployeeBuilder WithId(Guid id) { _id = id; return this; }
    public EmployeeBuilder WithName(string name) { _name = name; return this; }
    public EmployeeBuilder WithCpf(string cpf) { _cpf = cpf; return this; }
    public EmployeeBuilder WithEmail(string email) { _email = email; return this; }
    public EmployeeBuilder WithSalary(decimal salary) { _salary = salary; return this; }
    public EmployeeBuilder WithPixKey(string pixKey, PixKeyType type) { _pixKey = pixKey; _pixKeyType = type; return this; }
    public EmployeeBuilder WithCompanyId(Guid companyId) { _companyId = companyId; return this; }
    public EmployeeBuilder WithDepartmentId(Guid departmentId) { _departmentId = departmentId; return this; }
    public EmployeeBuilder WithPositionId(Guid positionId) { _positionId = positionId; return this; }
    public EmployeeBuilder Inactive() { _active = false; return this; }

    public Employee Build()
    {
        var employee = new Employee(_name, _cpf, _email, _salary, _pixKeyType, _pixKey, _companyId, _departmentId, _positionId);
        var idProp = typeof(Employee).GetProperty(nameof(Employee.Id));
        idProp!.SetValue(employee, _id);
        var activeProp = typeof(Employee).GetProperty(nameof(Employee.Active));
        activeProp!.SetValue(employee, _active);
        return employee;
    }
}