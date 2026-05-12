using System.ComponentModel.DataAnnotations;

namespace VehiclePartsPro.Domain.Entities;

public class Part
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string PartNumber { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Description { get; set; }

    public decimal UnitPrice { get; set; }

    public int StockQuantity { get; set; }

    public int LowStockThreshold { get; set; } = 10;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}