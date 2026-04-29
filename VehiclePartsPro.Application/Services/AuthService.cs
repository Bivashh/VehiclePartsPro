using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VehiclePartsPro.Infrastructure.Data;
using VehiclePartsPro.Application.DTOs.Auth;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;

namespace VehiclePartsPro.Application.Services;

/// <summary>
/// Handles registration and login using ASP.NET Core Identity.
///
/// 
/// Instead of manually hashing passwords (BCrypt), Identity's UserManager
/// does it for us via CreateAsync(user, password).
/// Instead of a custom "Role" string column, Identity manages roles
/// in the AspNetRoles and AspNetUserRoles tables via RoleManager and UserManager.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        AppDbContext db,
        IConfiguration config,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _db = db;
        _config = config;
        _logger = logger;
    }

    // ── Feature 12: Customer self-registration ─────────────────────────────────

    public async Task<TokenResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto)
    {
        // Build an IdentityUser — UserName must be set (we use email as username)
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email   // Identity requires UserName; using email keeps it simple
        };

        // Identity hashes the password automatically 
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            // Identity returns detailed error messages (e.g. "Password too short")
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        // Assign "Customer" role — seeded in AppDbContext.OnModelCreating
        await _userManager.AddToRoleAsync(user, "Customer");

        // Create customer profile linked to the identity user
        var customer = new Customer
        {
            UserId = user.Id,   // Identity Id is a string (GUID)
            Phone = dto.Phone,
            Address = dto.Address
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        _logger.LogInformation("New customer registered: {Email}", user.Email);

        return await BuildTokenAsync(user);
    }

    // ── Feature 2: Admin registers staff ──────────────────────────────────────

    public async Task<TokenResponseDto> RegisterStaffAsync(RegisterStaffDto dto)
    {
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        await _userManager.AddToRoleAsync(user, "Staff");

        var staff = new Staff
        {
            UserId = user.Id,
            Phone = dto.Phone,
            EmployeeCode = dto.EmployeeCode
        };

        _db.Staff.Add(staff);
        await _db.SaveChangesAsync();

        _logger.LogInformation("New staff registered: {Email}", user.Email);

        return await BuildTokenAsync(user);
    }

    // ── Login: all roles share one endpoint ───────────────────────────────────

    public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
    {
        // FindByEmailAsync is provided by Identity — searches AspNetUsers table
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        // CheckPasswordAsync uses Identity's built-in password hasher
        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
            throw new UnauthorizedAccessException("Invalid email or password.");

        _logger.LogInformation("User logged in: {Email}", user.Email);

        return await BuildTokenAsync(user);
    }

    // ── JWT token builder ──────────────────────────────────────────────────────

    /// <summary>
    /// Creates a signed JWT token containing the user's ID, email, name, and role.
    /// The role comes from Identity's role system (AspNetUserRoles table).
    ///
    /// Why JWT? The client stores this token and sends it with every request.
    /// The API reads the token to know who is calling — without a DB lookup each time.
    /// </summary>
    private async Task<TokenResponseDto> BuildTokenAsync(User user)
    {
        var jwtKey = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is missing from appsettings.json");

        // Get the user's roles from Identity (e.g. ["Admin"] or ["Customer"])
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Customer";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, role)          // this is what [Authorize(Roles="Admin")] checks
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new TokenResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Role = role,
            FullName = user.FullName,
            UserId = user.Id
        };
    }
}