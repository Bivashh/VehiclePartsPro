using System.ComponentModel.DataAnnotations;

namespace VehiclePartsPro.Domain.Entities;

public class Review
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    [Required]
    public int AppointmentId { get; set; }

    public Appointment Appointment { get; set; } = null!;

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}