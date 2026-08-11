using Microsoft.Extensions.DependencyInjection;
using PeopleBank.Application.UseCases.Account;
using PeopleBank.Application.UseCases.Benefits;
using PeopleBank.Application.UseCases.Company;
using PeopleBank.Application.UseCases.Department;
using PeopleBank.Application.UseCases.Employee;
using PeopleBank.Application.UseCases.Payroll;
using PeopleBank.Application.UseCases.Pix;
using PeopleBank.Application.UseCases.Position;
using PeopleBank.Application.UseCases.TimeEntry;

namespace PeopleBank.Application;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddUseCases();

        return services;
    }

    private static void AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IRegisterCompanyUseCase, RegisterCompanyUseCase>();
        services.AddScoped<IAddEmployeeUseCase, AddEmployeeUseCase>();
        services.AddScoped<IOpenAccountUseCase, OpenAccountUseCase>();
        services.AddScoped<IGetStatementUseCase, GetStatementUseCase>();
        services.AddScoped<IRegisterTimeEntryUseCase, RegisterTimeEntryUseCase>();
        services.AddScoped<IProcessPayrollUseCase, ProcessPayrollUseCase>();
        services.AddScoped<IRequestPixUseCase, RequestPixUseCase>();
        services.AddScoped<IDefineBenefitUseCase, DefineBenefitUseCase>();
        services.AddScoped<ISpendBenefitUseCase, SpendBenefitUseCase>();
        services.AddScoped<ICreateDepartmentUseCase, CreateDepartmentUseCase>();
        services.AddScoped<ICreatePositionUseCase, CreatePositionUseCase>();
    }
}