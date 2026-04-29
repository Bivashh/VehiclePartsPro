using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclePartsPro.Application.DTOs.Customer;
using VehiclePartsPro.Application.DTOs.Staff;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

// ── Feature 2: Admin manages staff ────────────────────────────────────────────

[ApiController]
[Route("api/staff")]
[Authorize(Roles = "Admin")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly ILogger<StaffController> _logger;

    public StaffController(IStaffService staffService, ILogger<StaffController> logger)
    {
        _staffService = staffService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _staffService.GetAllStaffAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _staffService.GetStaffByIdAsync(id);
        return result is null ? NotFound($"Staff {id} not found.") : Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStaffDto dto)
    {
        var result = await _staffService.UpdateStaffAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _staffService.DeleteStaffAsync(id);
        return Ok(new { Message = "Staff account deactivated." });
    }
}

// ── Features 8, 10, 12: Customer management ───────────────────────────────────

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// Feature 8: Staff/Admin — get all customers
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAll() =>
        Ok(await _customerService.GetAllCustomersAsync());

    /// Feature 8: Staff/Admin — get single customer with vehicle info
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _customerService.GetCustomerByIdAsync(id);
        return result is null ? NotFound($"Customer {id} not found.") : Ok(result);
    }

    /// Feature 12: Customer views their own profile
    [HttpGet("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _customerService.GetCustomerByUserIdAsync(userId);
        return result is null ? NotFound("Profile not found.") : Ok(result);
    }

    /// Feature 12: Customer updates their own profile
    [HttpPut("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateCustomerProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _logger.LogInformation("Customer {UserId} updating profile", userId);
        var result = await _customerService.UpdateCustomerProfileAsync(userId, dto);
        return Ok(result);
    }

    /// Feature 10: Search customers by name/phone/email/plate/ID
    [HttpGet("search")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Search([FromQuery] CustomerSearchDto search) =>
        Ok(await _customerService.SearchCustomersAsync(search));
}