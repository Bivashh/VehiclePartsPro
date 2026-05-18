namespace VehiclePartsPro.Application.DTOs.LowStockAlert;

public class LowStockAlertDto
{
    public int Id { get; set; }

    public int PartId { get; set; }

    public string PartName { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;

    public int CurrentStock { get; set; }

    public int Threshold { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsResolved { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }
}