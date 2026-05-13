using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehiclePartsPro.Domain.Entities;

public class SalesInvoiceItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SalesInvoiceId { get; set; }

    public SalesInvoice SalesInvoice { get; set; } = null!;

    [Required]
    public int PartId { get; set; }

    public Part Part { get; set; } = null!;

    [Required]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTotal { get; set; }
}