using Microsoft.EntityFrameworkCore;
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
        modelBuilder.Ignore<Cpf>();
        modelBuilder.Ignore<Cnpj>();
        modelBuilder.Ignore<Email>();

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Cnpj)
                  .HasConversion(c => c.Value, value => new Cnpj(value))
                  .HasMaxLength(14)
                  .IsRequired();
            entity.HasIndex(c => c.Cnpj).IsUnique();
            entity.HasMany(c => c.Departments).WithOne(d => d.Company).HasForeignKey(d => d.CompanyId);
            entity.HasMany(c => c.Employees).WithOne(e => e.Company).HasForeignKey(e => e.CompanyId);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Title).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Cpf)
                  .HasConversion(c => c.Value, value => new Cpf(value))
                  .HasMaxLength(11)
                  .IsRequired();
            entity.HasIndex(e => e.Cpf).IsUnique();
            entity.Property(e => e.Email)
                  .HasConversion(e => e.Value, value => new Email(value))
                  .HasMaxLength(200)
                  .IsRequired();
            entity.Property(e => e.Salary).HasPrecision(18, 2);
            entity.Property(e => e.PixKey).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PixKeyType).HasConversion<int>();

            entity.HasOne(e => e.Department).WithMany(d => d.Employees).HasForeignKey(e => e.DepartmentId);
            entity.HasOne(e => e.Position).WithMany(p => p.Employees).HasForeignKey(e => e.PositionId);
            entity.HasOne(e => e.Account).WithOne(a => a.Employee).HasForeignKey<Account>(a => a.EmployeeId);
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.PixKey).HasMaxLength(100).IsRequired();
            entity.Property(a => a.PixKeyType).HasConversion<int>();
            entity.Property(a => a.Balance).HasPrecision(18, 2);
            entity.Property(a => a.BlockedBalance).HasPrecision(18, 2);
            entity.Ignore(a => a.AvailableBalance);
            entity.HasMany(a => a.Transactions).WithOne(t => t.SourceAccount).HasForeignKey(t => t.SourceAccountId);
            entity.HasMany(a => a.BenefitWallets).WithOne(bw => bw.Account).HasForeignKey(bw => bw.AccountId);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).HasPrecision(18, 2);
            entity.Property(t => t.Status).HasConversion<int>();
            entity.Property(t => t.Description).HasMaxLength(500);
            entity.HasIndex(t => t.IdempotencyKey);
            entity.HasOne(t => t.TargetAccount).WithMany().HasForeignKey(t => t.TargetAccountId);
        });

        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Type).HasConversion<int>();
            entity.HasOne(t => t.Employee).WithMany(e => e.TimeEntries).HasForeignKey(t => t.EmployeeId);
        });

        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Status).HasConversion<int>();
            entity.HasOne(p => p.Company).WithMany().HasForeignKey(p => p.CompanyId);
            entity.HasMany(p => p.Payslips).WithOne(ps => ps.Payroll).HasForeignKey(ps => ps.PayrollId);
        });

        modelBuilder.Entity<Payslip>(entity =>
        {
            entity.HasKey(ps => ps.Id);
            entity.Property(ps => ps.BaseSalary).HasPrecision(18, 2);
            entity.Property(ps => ps.OvertimeAmount).HasPrecision(18, 2);
            entity.Property(ps => ps.Discounts).HasPrecision(18, 2);
            entity.Property(ps => ps.NetSalary).HasPrecision(18, 2);
            entity.HasOne(ps => ps.Employee).WithMany().HasForeignKey(ps => ps.EmployeeId);
        });

        modelBuilder.Entity<BenefitDefinition>(entity =>
        {
            entity.HasKey(bd => bd.Id);
            entity.Property(bd => bd.Name).HasMaxLength(100).IsRequired();
            entity.Property(bd => bd.Category).HasConversion<int>();
            entity.Property(bd => bd.MonthlyAmount).HasPrecision(18, 2);
            entity.HasOne(bd => bd.Company).WithMany().HasForeignKey(bd => bd.CompanyId);
        });

        modelBuilder.Entity<BenefitWallet>(entity =>
        {
            entity.HasKey(bw => bw.Id);
            entity.Property(bw => bw.Balance).HasPrecision(18, 2);
            entity.HasOne(bw => bw.BenefitDefinition).WithMany().HasForeignKey(bw => bw.BenefitDefinitionId);
        });

        base.OnModelCreating(modelBuilder);
    }
}