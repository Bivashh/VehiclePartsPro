using System.ComponentModel.DataAnnotations;

namespace VehiclePartsPro.Domain.Entities;

public class Customer
{
    [Key]
    public int Id { get; set; }

    // FK to Identity user
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    public decimal CreditBalance { get; set; } = 0;
    public decimal TotalSpent { get; set; } = 0;

    public DateTime? CreditDueDate { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}