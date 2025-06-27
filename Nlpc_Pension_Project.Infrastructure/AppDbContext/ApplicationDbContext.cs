using Microsoft.EntityFrameworkCore;
using Nlpc_Pension_Project.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Nlpc_Pension_Project.Infrastructure.AppDbContext;

// ApplicationDbContext.cs
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members { get; set; }
    public DbSet<Contribution> Contributions { get; set; }
    public DbSet<Benefit> Benefits { get; set; }
    public DbSet<Employer> Employers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Member>(entity =>
        {
            entity.Property(m => m.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(m => m.LastName).IsRequired().HasMaxLength(50);
            entity.Property(m => m.Email).IsRequired().HasMaxLength(100);
            entity.Property(m => m.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<Contribution>(entity =>
        {
            entity.Property(c => c.ReferenceNumber).IsRequired();

            // Fix: Define precision for Amount
            entity.Property(c => c.Amount)
                //.HasColumnType("decimal(18,2)") // Optional: explicit SQL type
                .HasPrecision(18, 2); // 18 digits total, 2 after decimal
        });

        modelBuilder.Entity<Benefit>(entity =>
        {
            // Fix: Define precision for Amount
            entity.Property(b => b.Amount)
                //.HasColumnType("decimal(18,2)") // Optional: explicit SQL type
                .HasPrecision(18, 2); // 18 digits total, 2 after decimal
        });

        modelBuilder.Entity<Employer>(entity =>
        {
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(50);
        });
    }
}
