using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Enums;

namespace CommonTestUtilities.Requests;

public class AddEmployeeRequestJsonBuilder
{
    public static AddEmployeeRequestJson Build() => new()
    {
        Name = "John",
        Cpf = "12345678901",
        Email = "john@example.com",
        Salary = 5000,
        PixKeyType = PixKeyType.CPF,
        PixKey = "12345678901",
        CompanyId = Guid.NewGuid(),
        DepartmentId = Guid.NewGuid(),
        PositionId = Guid.NewGuid()
    };
}
