using System.Data;
using FluentMigrator;

namespace PeopleBank.Infrastructure.Migrations.Versions;

[Migration(DatabaseVersions.INITIAL_TABLES, "Creating initials table")]
public class Versions0000001 : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.Table("Company")
           .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
           .WithColumn("Name").AsString(200).NotNullable()
           .WithColumn("Cnpj").AsString(14).NotNullable().Unique()
           .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
           .WithColumn("Active").AsBoolean().NotNullable().WithDefaultValue(true);

        Create.Table("Department")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("CompanyId").AsGuid().NotNullable()
            .ForeignKey("FK_Department_Company", "Company", "Id")
                .OnDelete(Rule.None);

        Create.Table("Position")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Title").AsString(100).NotNullable()
            .WithColumn("CompanyId").AsGuid().NotNullable()
            .ForeignKey("FK_Position_Company", "Company", "Id")
                .OnDelete(Rule.None);

        Create.Table("Employee")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Cpf").AsString(11).NotNullable().Unique()
            .WithColumn("Email").AsString(200).NotNullable()
            .WithColumn("Salary").AsDecimal(18, 2).NotNullable()
            .WithColumn("PixKey").AsString(100).NotNullable()
            .WithColumn("PixKeyType").AsInt32().NotNullable()
            .WithColumn("CompanyId").AsGuid().NotNullable()
            .WithColumn("DepartmentId").AsGuid().NotNullable()
            .WithColumn("PositionId").AsGuid().NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("Active").AsBoolean().NotNullable().WithDefaultValue(true);

        Create.ForeignKey("FK_Employee_Company")
            .FromTable("Employee").ForeignColumn("CompanyId")
            .ToTable("Company").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.ForeignKey("FK_Employee_Department")
            .FromTable("Employee").ForeignColumn("DepartmentId")
            .ToTable("Department").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.ForeignKey("FK_Employee_Position")
            .FromTable("Employee").ForeignColumn("PositionId")
            .ToTable("Position").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.Table("Account")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("EmployeeId").AsGuid().NotNullable().Unique() 
            .WithColumn("PixKey").AsString(100).NotNullable()
            .WithColumn("PixKeyType").AsInt32().NotNullable()
            .WithColumn("Balance").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("BlockedBalance").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("Active").AsBoolean().NotNullable().WithDefaultValue(true);

        Create.ForeignKey("FK_Account_Employee")
            .FromTable("Account").ForeignColumn("EmployeeId")
            .ToTable("Employee").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.Table("Transaction")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("SourceAccountId").AsGuid().NotNullable()
            .WithColumn("TargetAccountId").AsGuid().Nullable()
            .WithColumn("Amount").AsDecimal(18, 2).NotNullable()
            .WithColumn("Status").AsInt32().NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("ProcessedAt").AsDateTime().Nullable()
            .WithColumn("ErrorMessage").AsString(500).Nullable()
            .WithColumn("IdempotencyKey").AsString(100).NotNullable()
            .WithColumn("Description").AsString(500).Nullable();

        Create.Index("IX_Transaction_IdempotencyKey")
            .OnTable("Transaction")
            .OnColumn("IdempotencyKey").Ascending();

        Create.ForeignKey("FK_Transaction_SourceAccount")
            .FromTable("Transaction").ForeignColumn("SourceAccountId")
            .ToTable("Account").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.ForeignKey("FK_Transaction_TargetAccount")
            .FromTable("Transaction").ForeignColumn("TargetAccountId")
            .ToTable("Account").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.Table("TimeEntry")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("EmployeeId").AsGuid().NotNullable()
            .WithColumn("Timestamp").AsDateTime().NotNullable()
            .WithColumn("Type").AsInt32().NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime);

        Create.ForeignKey("FK_TimeEntry_Employee")
            .FromTable("TimeEntry").ForeignColumn("EmployeeId")
            .ToTable("Employee").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.Table("Payroll")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("CompanyId").AsGuid().NotNullable()
            .WithColumn("Month").AsInt32().NotNullable()
            .WithColumn("Year").AsInt32().NotNullable()
            .WithColumn("Status").AsInt32().NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("ProcessedAt").AsDateTime().Nullable();

        Create.ForeignKey("FK_Payroll_Company")
            .FromTable("Payroll").ForeignColumn("CompanyId")
            .ToTable("Company").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.Table("Payslip")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("PayrollId").AsGuid().NotNullable()
            .WithColumn("EmployeeId").AsGuid().NotNullable()
            .WithColumn("BaseSalary").AsDecimal(18, 2).NotNullable()
            .WithColumn("OvertimeHours").AsInt32().NotNullable()
            .WithColumn("OvertimeAmount").AsDecimal(18, 2).NotNullable()
            .WithColumn("Discounts").AsDecimal(18, 2).NotNullable()
            .WithColumn("NetSalary").AsDecimal(18, 2).NotNullable()
            .WithColumn("PaidAt").AsDateTime().Nullable();

        Create.ForeignKey("FK_Payslip_Payroll")
            .FromTable("Payslip").ForeignColumn("PayrollId")
            .ToTable("Payroll").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.ForeignKey("FK_Payslip_Employee")
            .FromTable("Payslip").ForeignColumn("EmployeeId")
            .ToTable("Employee").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.Table("BenefitDefinition")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("CompanyId").AsGuid().NotNullable()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("Category").AsInt32().NotNullable()
            .WithColumn("MonthlyAmount").AsDecimal(18, 2).NotNullable()
            .WithColumn("Active").AsBoolean().NotNullable().WithDefaultValue(true);

        Create.ForeignKey("FK_BenefitDefinition_Company")
            .FromTable("BenefitDefinition").ForeignColumn("CompanyId")
            .ToTable("Company").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.Table("BenefitWallet")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("AccountId").AsGuid().NotNullable()
            .WithColumn("BenefitDefinitionId").AsGuid().NotNullable()
            .WithColumn("Balance").AsDecimal(18, 2).NotNullable()
            .WithColumn("ExpirationDate").AsDateTime().NotNullable();

        Create.ForeignKey("FK_BenefitWallet_Account")
            .FromTable("BenefitWallet").ForeignColumn("AccountId")
            .ToTable("Account").PrimaryColumn("Id")
            .OnDelete(Rule.None);

        Create.ForeignKey("FK_BenefitWallet_BenefitDefinition")
            .FromTable("BenefitWallet").ForeignColumn("BenefitDefinitionId")
            .ToTable("BenefitDefinition").PrimaryColumn("Id")
            .OnDelete(Rule.None);
    }
}
