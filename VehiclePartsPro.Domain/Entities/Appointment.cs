using System.ComponentModel.DataAnnotations;

namespace VehiclePartsPro.Domain.Entities;

public class Appointment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    [Required]
    public int VehicleId { get; set; }

    public Vehicle Vehicle { get; set; } = null!;

    // Optional assigned staff
    public int? StaffId { get; set; }

    public Staff? Staff { get; set; }

    [Required]
    [MaxLength(100)]
    public string ServiceType { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime AppointmentDate { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}