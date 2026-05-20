using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehiclePartsPro.Application.DTOs.Customer;
using VehiclePartsPro.Application.DTOs.Staff;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/staff")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly ICustomerService _customerService;
    private readonly IOverduePaymentReminderService _overduePaymentReminderService;

    public StaffController(
        IStaffService staffService,
        ICustomerService customerService,
        IOverduePaymentReminderService overduePaymentReminderService)
    {
        _staffService = staffService;
        _customerService = customerService;
        _overduePaymentReminderService = overduePaymentReminderService;
    }

    
    // ADMIN → GET ALL STAFF
   
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var staffs = await _staffService.GetAllStaffAsync();
        return Ok(staffs);
    }

    
    // STAFF → GET OWN PROFILE
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

    
    // STAFF → UPDATE OWN PROFILE
    
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

        staff.Phone = dto.Phone;

        await _staffService.UpdateStaffAsync(staff);

        return Ok("Staff profile updated successfully");
    }

    
    // ADMIN → DELETE STAFF
    
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

    
    // STAFF/ADMIN → REGISTER CUSTOMER WITH VEHICLE
    
    [HttpPost("customers/register-with-vehicle")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> RegisterCustomerWithVehicle(RegisterCustomerWithVehicleDto dto)
    {
        var result = await _customerService.RegisterCustomerWithVehicleAsync(dto);

        return Ok(result);
    }

    
    // STAFF/ADMIN → SEND OVERDUE PAYMENT REMINDERS
    
    [HttpPost("customers/send-overdue-reminders")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> SendOverduePaymentReminders()
    {
        var result = await _overduePaymentReminderService.SendOverduePaymentRemindersAsync();

        return Ok(new
        {
            message = "Overdue payment reminder process completed.",
            totalCustomersReminded = result.Count,
            customers = result
        });
    }
}