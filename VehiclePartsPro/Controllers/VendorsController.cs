using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclePartsPro.Application.DTOs.Vendor;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/vendors")]
[Authorize(Roles = "Admin")]
public class VendorsController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorsController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vendors = await _vendorService.GetAllVendorsAsync();
        return Ok(vendors);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(id);

        if (vendor == null)
        {
            return NotFound(new { message = $"Vendor with ID {id} not found." });
        }

        return Ok(vendor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVendorDto dto)
    {
        var result = await _vendorService.CreateVendorAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVendorDto dto)
    {
        var result = await _vendorService.UpdateVendorAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _vendorService.DeleteVendorAsync(id);
        return Ok(new { message = "Vendor deleted successfully." });
    }
}