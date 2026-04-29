using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace VehiclePartsPro.Domain.Entities;

public class User : IdentityUser
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // One-to-one navigation
    public Staff? Staff { get; set; }
    public Customer? Customer { get; set; }
}

//using Microsoft.AspNetCore.Identity;
//using System.ComponentModel.DataAnnotations;

//namespace VehiclePartsPro.Domain.Entities;

//public class User : IdentityUser
//{
//    [Required]
//    [MaxLength(150)]
//    public string FullName { get; set; } = string.Empty;

//    [Required]
//    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//    // Navigation — one user can be a Staff or Customer profile
//    public Staff? Staff { get; set; }
//    public Customer? Customer { get; set; }
//}