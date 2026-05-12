using System.ComponentModel.DataAnnotations;

namespace VehiclePartsPro.Domain.Entities;

public class PurchaseInvoiceItem
{
    [Key]
    public int Id { get; set; }

    public int PurchaseInvoiceId { get; set; }

    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;

    public int PartId { get; set; }

    public Part Part { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public decimal LineTotal { get; set; }
}