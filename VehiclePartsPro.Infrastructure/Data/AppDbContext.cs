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

    public DbSet<SalesInvoice> SalesInvoices { get; set; }
    public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }
    public DbSet<LowStockAlert> LowStockAlerts { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<PartRequest> PartRequests { get; set; }
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

        builder.Entity<SalesInvoice>()
            .HasOne(si => si.Customer)
            .WithMany()
            .HasForeignKey(si => si.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SalesInvoiceItem>()
            .HasOne(sii => sii.SalesInvoice)
            .WithMany(si => si.Items)
            .HasForeignKey(sii => sii.SalesInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<SalesInvoiceItem>()
            .HasOne(sii => sii.Part)
            .WithMany()
            .HasForeignKey(sii => sii.PartId)
            .OnDelete(DeleteBehavior.Restrict);

        // Low Stock Alert configuration
        builder.Entity<LowStockAlert>()
            .HasOne(alert => alert.Part)
            .WithMany()
            .HasForeignKey(alert => alert.PartId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional: enforce UserId uniqueness (1:1 logical constraint)
        builder.Entity<Customer>()
            .HasIndex(c => c.UserId)
            .IsUnique();

        builder.Entity<Staff>()
            .HasIndex(s => s.UserId)
            .IsUnique();

        builder.Entity<Appointment>()
            .HasOne(a => a.Customer)
            .WithMany()
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Vehicle)
            .WithMany()
            .HasForeignKey(a => a.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Staff)
            .WithMany()
            .HasForeignKey(a => a.StaffId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Review>()
            .HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Review>()
            .HasOne(r => r.Appointment)
            .WithMany()
            .HasForeignKey(r => r.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // ONE REVIEW PER APPOINTMENT
        builder.Entity<Review>()
            .HasIndex(r => r.AppointmentId)
            .IsUnique();

        builder.Entity<PartRequest>()
            .HasOne(pr => pr.Customer)
            .WithMany()
            .HasForeignKey(pr => pr.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}