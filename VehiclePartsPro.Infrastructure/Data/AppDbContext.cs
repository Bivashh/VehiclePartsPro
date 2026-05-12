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
    public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }

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

        // Purchase Invoice configuration
        builder.Entity<PurchaseInvoice>()
            .HasIndex(pi => pi.InvoiceNumber)
            .IsUnique();

        builder.Entity<PurchaseInvoice>()
            .Property(pi => pi.TotalAmount)
            .HasColumnType("decimal(18,2)");

        builder.Entity<PurchaseInvoice>()
            .HasOne(pi => pi.Vendor)
            .WithMany()
            .HasForeignKey(pi => pi.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Purchase Invoice Item configuration
        builder.Entity<PurchaseInvoiceItem>()
            .Property(item => item.UnitCost)
            .HasColumnType("decimal(18,2)");

        builder.Entity<PurchaseInvoiceItem>()
            .Property(item => item.LineTotal)
            .HasColumnType("decimal(18,2)");

        builder.Entity<PurchaseInvoiceItem>()
            .HasOne(item => item.PurchaseInvoice)
            .WithMany(pi => pi.Items)
            .HasForeignKey(item => item.PurchaseInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PurchaseInvoiceItem>()
            .HasOne(item => item.Part)
            .WithMany()
            .HasForeignKey(item => item.PartId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional: enforce UserId uniqueness (1:1 logical constraint)
        builder.Entity<Customer>()
            .HasIndex(c => c.UserId)
            .IsUnique();

        builder.Entity<Staff>()
            .HasIndex(s => s.UserId)
            .IsUnique();
    }
}