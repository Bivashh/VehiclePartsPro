using System.ComponentModel.DataAnnotations;

namespace VehiclePartsPro.Domain.Entities;

public class PurchaseInvoice
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public int VendorId { get; set; }

    public Vendor Vendor { get; set; } = null!;

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<PurchaseInvoiceItem> Items { get; set; } = new();
}