using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehiclePartsPro.Application.DTOs.Staff;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/staff")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    // =========================================
    // ADMIN → GET ALL STAFF
    // =========================================
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var staffs = await _staffService.GetAllStaffAsync();

        return Ok(staffs);
    }

    // =========================================
    // STAFF → GET OWN PROFILE
    // =========================================
    [HttpGet("me")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        var staff = await _staffService.GetStaffByUserIdAsync(userId);

        if (staff == null)
            return NotFound("Staff profile not found");

        return Ok(staff);
    }

    // =========================================
    // STAFF → UPDATE OWN PROFILE
    // =========================================
    [HttpPut("me")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> UpdateProfile(UpdateStaffDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        var staff = await _staffService.GetStaffByUserIdAsync(userId);

        if (staff == null)
            return NotFound("Staff profile not found");

        // Only update editable fields
        staff.Phone = dto.Phone;

        await _staffService.UpdateStaffAsync(staff);

        return Ok("Staff profile updated successfully");
    }

    // =========================================
    // ADMIN → DELETE STAFF
    // =========================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var staff = await _staffService.GetStaffByIdAsync(id);

        if (staff == null)
            return NotFound();

        await _staffService.DeleteStaffAsync(staff);

        return Ok("Staff deleted successfully");
    }
}