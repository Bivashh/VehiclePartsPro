using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VehiclePartsPro.Domain.Entities;

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Staff → User (1:1)
        builder.Entity<Staff>()
            .HasOne(s => s.User)
            .WithOne(u => u.Staff)
            .HasForeignKey<Staff>(s => s.UserId);

        // Customer → User (1:1)
        builder.Entity<Customer>()
            .HasOne(c => c.User)
            .WithOne(u => u.Customer)
            .HasForeignKey<Customer>(c => c.UserId);

        // Customer → Vehicles (1:M)
        builder.Entity<Vehicle>()
            .HasOne(v => v.Customer)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(v => v.CustomerId);

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



    }
}