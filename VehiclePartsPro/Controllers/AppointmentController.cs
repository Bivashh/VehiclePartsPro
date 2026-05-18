using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehiclePartsPro.Application.DTOs.Appointment;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    // CUSTOMER → CREATE
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create(CreateAppointmentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _appointmentService
            .CreateAppointmentAsync(userId, dto);

        return Ok(result);
    }

    // CUSTOMER → MY APPOINTMENTS
    [HttpGet("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _appointmentService
            .GetMyAppointmentsAsync(userId);

        return Ok(result);
    }

    // ADMIN/STAFF → ALL
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _appointmentService
            .GetAllAppointmentsAsync();

        return Ok(result);
    }

    // ADMIN/STAFF → UPDATE STATUS
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateAppointmentStatusDto dto)
    {
        var appointment = await _appointmentService
            .GetAppointmentEntityAsync(id);

        if (appointment == null)
            return NotFound();

        await _appointmentService
            .UpdateAppointmentStatusAsync(appointment, dto);

        return Ok(new
        {
            message = "Appointment updated successfully"
        });
    }

    // CUSTOMER → CANCEL OWN
    [HttpPut("{id}/cancel")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Cancel(int id)
    {
        var appointment = await _appointmentService
            .GetAppointmentEntityAsync(id);

        if (appointment == null)
            return NotFound();

        await _appointmentService
            .CancelAppointmentAsync(appointment);

        return Ok(new
        {
            message = "Appointment cancelled successfully"
        });
    }
}