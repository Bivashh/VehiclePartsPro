using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Identity;

namespace VehiclePartsPro.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    // DbSets
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Part> Parts { get; set; }
    public DbSet<Vendor> Vendors { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ❌ REMOVE User navigation mappings (they no longer exist)

        // Customer → Vehicles (1:M)
        builder.Entity<Vehicle>()
            .HasOne(v => v.Customer)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(v => v.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Money precision
        builder.Entity<Customer>()
            .Property(c => c.CreditBalance)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Customer>()
            .Property(c => c.TotalSpent)
            .HasColumnType("decimal(18,2)");

        // Part configuration
        builder.Entity<Part>()
            .HasIndex(p => p.PartNumber)
            .IsUnique();

        builder.Entity<Part>()
            .Property(p => p.UnitPrice)
            .HasColumnType("decimal(18,2)");

        // Vendor configuration
        builder.Entity<Vendor>()
            .HasIndex(v => v.Email)
            .IsUnique();

        // Optional: enforce UserId uniqueness (1:1 logical constraint)
        builder.Entity<Customer>()
            .HasIndex(c => c.UserId)
            .IsUnique();

        builder.Entity<Staff>()
            .HasIndex(s => s.UserId)
            .IsUnique();
    }
}