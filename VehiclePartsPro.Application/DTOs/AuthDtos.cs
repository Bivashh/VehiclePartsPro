namespace VehiclePartsPro.Application.DTOs.Auth;

// What the client sends to register as a customer
public class RegisterCustomerDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
}




// What the client sends to log in (same for all roles)
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// What we send back after successful login or register
public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}