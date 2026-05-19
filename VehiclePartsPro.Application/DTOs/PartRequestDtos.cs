namespace VehiclePartsPro.Application.DTOs.PartRequest;

public class CreatePartRequestDto
{
    public string PartName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdatePartRequestStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class PartRequestDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string PartName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}