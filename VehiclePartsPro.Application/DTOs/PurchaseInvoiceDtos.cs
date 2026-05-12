using System.ComponentModel.DataAnnotations;

namespace VehiclePartsPro.Application.DTOs.PurchaseInvoice;

public class PurchaseInvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public List<PurchaseInvoiceItemDto> Items { get; set; } = new();
}

public class PurchaseInvoiceItemDto
{
    public int Id { get; set; }
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal { get; set; }
}

public class CreatePurchaseInvoiceDto
{
    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    public int VendorId { get; set; }

    public DateTime? PurchaseDate { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one invoice item is required.")]
    public List<CreatePurchaseInvoiceItemDto> Items { get; set; } = new();
}

public class CreatePurchaseInvoiceItemDto
{
    [Required]
    public int PartId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }

    [Range(0.01, 999999999, ErrorMessage = "Unit cost must be greater than 0.")]
    public decimal UnitCost { get; set; }
}