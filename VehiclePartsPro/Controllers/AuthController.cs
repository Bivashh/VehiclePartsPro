using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclePartsPro.Application.DTOs.Auth;
using VehiclePartsPro.Application.DTOs.Staff;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    
    // REGISTER CUSTOMER
    
    [HttpPost("register-customer")]
    public async Task<IActionResult> RegisterCustomer(
        [FromBody] RegisterCustomerDto dto)
    {
        var result = await _authService.RegisterCustomerAsync(dto);

        return Ok(result);
    }

    
    // REGISTER STAFF
    
    [HttpPost("register-staff")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterStaff(
        [FromBody] RegisterStaffDto dto)
    {
        var result = await _authService.RegisterStaffAsync(dto);

        return Ok(result);
    }

    
    // LOGIN
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        return Ok(result);
    }
}