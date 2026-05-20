using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehiclePartsPro.Application.DTOs.Customer;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // =========================
    // GET MY PROFILE
    // =========================
    [HttpGet("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _customerService.GetCustomerByUserIdAsync(userId);

        return result == null ? NotFound() : Ok(result);
    }

    // =========================
    // UPDATE PROFILE
    // =========================
    [HttpPut("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateProfile(UpdateCustomerProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var customer = await _customerService.GetCustomerByUserIdAsync(userId);

        if (customer == null)
            return NotFound();

        customer.Phone = dto.Phone;
        customer.Address = dto.Address;

        await _customerService.UpdateCustomerAsync(customer);

        return Ok(new
        {
            message = "Profile updated successfully"
        });
    }

    // =========================
    // GET VEHICLES
    // =========================
    [HttpGet("vehicles")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetVehicles()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var vehicles = await _customerService.GetVehiclesAsync(userId);

        return Ok(vehicles);
    }

    // =========================
    // ADD VEHICLE
    // =========================
    [HttpPost("vehicles")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> AddVehicle(VehicleDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var customer = await _customerService.GetCustomerByUserIdAsync(userId);

        if (customer == null)
            return NotFound();

        var vehicle = new Vehicle
        {
            CustomerId = customer.Id,
            PlateNumber = dto.PlateNumber,
            Make = dto.Make,
            Model = dto.Model,
            Year = dto.Year,
            Notes = dto.Notes
        };

        await _customerService.AddVehicleAsync(vehicle);

        return Ok(new
        {
            message = "Vehicle added successfully"
        });
    }

    // =========================
    // UPDATE VEHICLE
    // =========================
    [HttpPut("vehicles/{id}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateVehicle(int id, VehicleDto dto)
    {
        var vehicle = await _customerService.GetVehicleByIdAsync(id);

        if (vehicle == null)
            return NotFound();

        vehicle.PlateNumber = dto.PlateNumber;
        vehicle.Make = dto.Make;
        vehicle.Model = dto.Model;
        vehicle.Year = dto.Year;
        vehicle.Notes = dto.Notes;

        await _customerService.UpdateVehicleAsync(vehicle);

        return Ok(new
        {
            message = "Vehicle updated successfully"
        });
    }

    // =========================
    // DELETE VEHICLE
    // =========================
    [HttpDelete("vehicles/{id}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var vehicle = await _customerService.GetVehicleByIdAsync(id);

        if (vehicle == null)
            return NotFound();

        await _customerService.DeleteVehicleAsync(vehicle);

        return Ok(new
        {
            message = "Vehicle deleted successfully"
        });
    }


    // ADMIN → CUSTOMER REPORTS
    
    [HttpGet("reports")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetCustomerReports()
    {
        var reports = await _customerService.GetCustomerReportsAsync();

        return Ok(reports);
    }

    
    // ADMIN → CUSTOMER HISTORY
    
    [HttpGet("{customerId}/history")]
    [Authorize(Roles = "Admin,Staff ")]
    public async Task<IActionResult> GetCustomerHistory(int customerId)
    {
        var history = await _customerService.GetCustomerHistoryAsync(customerId);

        return Ok(history);
    }

    
    // ADMIN → SEARCH CUSTOMERS
    
    [HttpPost("search")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> SearchCustomers(CustomerSearchDto dto)
    {
        var result = await _customerService.SearchCustomersAsync(dto);

        return Ok(result);
    }
}