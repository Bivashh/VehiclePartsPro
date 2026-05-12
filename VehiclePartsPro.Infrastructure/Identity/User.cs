using Microsoft.AspNetCore.Identity;

namespace VehiclePartsPro.Infrastructure.Identity;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; internal set; }
}