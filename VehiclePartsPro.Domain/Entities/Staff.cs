using System.ComponentModel.DataAnnotations;

namespace VehiclePartsPro.Domain.Entities;

public class Staff
{
    [Key]
    public int Id { get; set; }

    // FK to Identity user
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public DateTime HiredAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
}