using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.ValueObjects;

namespace PeopleBank.Infrastructure.Data;

public class PeopleBankDbContext : DbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
    public DbSet<Payroll> Payrolls => Set<Payroll>();
    public DbSet<Payslip> Payslips => Set<Payslip>();
    public DbSet<BenefitDefinition> BenefitDefinitions => Set<BenefitDefinition>();
    public DbSet<BenefitWallet> BenefitWallets => Set<BenefitWallet>();

    public PeopleBankDbContext(DbContextOptions<PeopleBankDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Value Converters explícitos para os Value Objects
        var cnpjConverter = new ValueConverter<Cnpj, string>(
            v => v.Value,
            v => new Cnpj(v)
        );

        var cpfConverter = new ValueConverter<Cpf, string>(
            v => v.Value,
            v => new Cpf(v)
        );

        var emailConverter = new ValueConverter<Email, string>(
            v => v.Value,
            v => new Email(v)
        );

        // Company
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Company");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Cnpj)
                  .HasConversion(cnpjConverter)
                  .HasMaxLength(14)
                  .IsRequired();
            entity.HasIndex(c => c.Cnpj).IsUnique();
            entity.HasMany(c => c.Departments).WithOne(d => d.Company).HasForeignKey(d => d.CompanyId);
            entity.HasMany(c => c.Employees).WithOne(e => e.Company).HasForeignKey(e => e.CompanyId);
        });

        // Department
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).HasMaxLength(100).IsRequired();
        });

        // Position
        modelBuilder.Entity<Position>(entity =>
        {
            entity.ToTable("Position");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Title).HasMaxLength(100).IsRequired();
        });

        // Employee
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Cpf)
                  .HasConversion(cpfConverter)
                  .HasMaxLength(11)
                  .IsRequired();
            entity.HasIndex(e => e.Cpf).IsUnique();
            entity.Property(e => e.Email)
                  .HasConversion(emailConverter)
                  .HasMaxLength(200)
                  .IsRequired();
            entity.Property(e => e.Salary).HasPrecision(18, 2);
            entity.Property(e => e.PixKey).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PixKeyType).HasConversion<int>();

            entity.HasOne(e => e.Department).WithMany(d => d.Employees).HasForeignKey(e => e.DepartmentId);
            entity.HasOne(e => e.Position).WithMany(p => p.Employees).HasForeignKey(e => e.PositionId);
            entity.HasOne(e => e.Account).WithOne(a => a.Employee).HasForeignKey<Account>(a => a.EmployeeId);
        });

        // Account
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.PixKey).HasMaxLength(100).IsRequired();
            entity.Property(a => a.PixKeyType).HasConversion<int>();
            entity.Property(a => a.Balance).HasPrecision(18, 2);
            entity.Property(a => a.BlockedBalance).HasPrecision(18, 2);
            entity.Ignore(a => a.AvailableBalance);
            entity.HasMany(a => a.Transactions).WithOne(t => t.SourceAccount).HasForeignKey(t => t.SourceAccountId);
            entity.HasMany(a => a.BenefitWallets).WithOne(bw => bw.Account).HasForeignKey(bw => bw.AccountId);
        });

        // Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).HasPrecision(18, 2);
            entity.Property(t => t.Status).HasConversion<int>();
            entity.Property(t => t.Description).HasMaxLength(500);
            entity.HasIndex(t => t.IdempotencyKey);
            entity.HasOne(t => t.TargetAccount).WithMany().HasForeignKey(t => t.TargetAccountId);
        });

        // TimeEntry
        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.ToTable("TimeEntry");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Type).HasConversion<int>();
            entity.HasOne(t => t.Employee).WithMany(e => e.TimeEntries).HasForeignKey(t => t.EmployeeId);
        });

        // Payroll
        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.ToTable("Payroll");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Status).HasConversion<int>();
            entity.HasOne(p => p.Company).WithMany().HasForeignKey(p => p.CompanyId);
            entity.HasMany(p => p.Payslips).WithOne(ps => ps.Payroll).HasForeignKey(ps => ps.PayrollId);
        });

        // Payslip
        modelBuilder.Entity<Payslip>(entity =>
        {
            entity.ToTable("Payslip");
            entity.HasKey(ps => ps.Id);
            entity.Property(ps => ps.BaseSalary).HasPrecision(18, 2);
            entity.Property(ps => ps.OvertimeAmount).HasPrecision(18, 2);
            entity.Property(ps => ps.Discounts).HasPrecision(18, 2);
            entity.Property(ps => ps.NetSalary).HasPrecision(18, 2);
            entity.HasOne(ps => ps.Employee).WithMany().HasForeignKey(ps => ps.EmployeeId);
        });

        // BenefitDefinition
        modelBuilder.Entity<BenefitDefinition>(entity =>
        {
            entity.ToTable("BenefitDefinition");
            entity.HasKey(bd => bd.Id);
            entity.Property(bd => bd.Name).HasMaxLength(100).IsRequired();
            entity.Property(bd => bd.Category).HasConversion<int>();
            entity.Property(bd => bd.MonthlyAmount).HasPrecision(18, 2);
            entity.HasOne(bd => bd.Company).WithMany().HasForeignKey(bd => bd.CompanyId);
        });

        // BenefitWallet
        modelBuilder.Entity<BenefitWallet>(entity =>
        {
            entity.ToTable("BenefitWallet");
            entity.HasKey(bw => bw.Id);
            entity.Property(bw => bw.Balance).HasPrecision(18, 2);
            entity.HasOne(bw => bw.BenefitDefinition).WithMany().HasForeignKey(bw => bw.BenefitDefinitionId);
        });

        base.OnModelCreating(modelBuilder);
    }
}