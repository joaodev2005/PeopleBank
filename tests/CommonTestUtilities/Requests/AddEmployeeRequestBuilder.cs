using PeopleBank.Communication.Requests;
using PeopleBank.Domain.Enums;

namespace PeopleBank.CommonTestUtilities.Requests;

public class AddEmployeeRequestBuilder
{
    public static AddEmployeeRequestJson Build(
        Guid? companyId = null,
        Guid? departmentId = null,
        Guid? positionId = null,
        string? cpf = null,
        string? email = null)
    {
        return new AddEmployeeRequestJson
        {
            Name = "John",
            Cpf = cpf ?? "52998224725",
            Email = email ?? "john@example.com",
            Salary = 5000,
            PixKeyType = PixKeyType.Email,
            PixKey = email ?? "john@example.com",
            CompanyId = companyId ?? Guid.NewGuid(),
            DepartmentId = departmentId ?? Guid.NewGuid(),
            PositionId = positionId ?? Guid.NewGuid()
        };
    }
}