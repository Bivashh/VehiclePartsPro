using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclePartsPro.Application.DTOs.Auth;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

/// <summary>
/// Auth endpoints:
///   POST /api/auth/register-customer  (public)
///   POST /api/auth/register-staff     (Admin only)
///   POST /api/auth/login              (public)
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// POST /api/auth/register-customer
    /// Feature 12: Customer self-registration (no auth required)
    [HttpPost("register-customer")]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerDto dto)
    {
        _logger.LogInformation("Customer registration attempt: {Email}", dto.Email);
        var result = await _authService.RegisterCustomerAsync(dto);
        return Ok(result);
    }

    /// POST /api/auth/register-staff
    /// Feature 2: Only Admin can create staff accounts
    [HttpPost("register-staff")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffDto dto)
    {
        _logger.LogInformation("Admin creating staff account: {Email}", dto.Email);
        var result = await _authService.RegisterStaffAsync(dto);
        return Ok(result);
    }

    /// POST /api/auth/login
    /// Login API — same endpoint for all roles
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }
}