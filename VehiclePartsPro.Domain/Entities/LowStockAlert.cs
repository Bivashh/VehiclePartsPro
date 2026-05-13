using System.ComponentModel.DataAnnotations;

namespace VehiclePartsPro.Domain.Entities;

public class LowStockAlert
{
    [Key]
    public int Id { get; set; }

    public int PartId { get; set; }

    public Part Part { get; set; } = null!;

    public int CurrentStock { get; set; }

    public int Threshold { get; set; }

    [MaxLength(300)]
    public string Message { get; set; } = string.Empty;

    public bool IsResolved { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ResolvedAt { get; set; }
}