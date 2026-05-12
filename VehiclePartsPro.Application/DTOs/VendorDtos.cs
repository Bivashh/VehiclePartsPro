using System.ComponentModel.DataAnnotations;

namespace VehiclePartsPro.Application.DTOs.Vendor;

public class VendorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
}

public class CreateVendorDto
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? ContactPerson { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(120)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(250)]
    public string? Address { get; set; }
}

public class UpdateVendorDto
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? ContactPerson { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(120)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(250)]
    public string? Address { get; set; }
}