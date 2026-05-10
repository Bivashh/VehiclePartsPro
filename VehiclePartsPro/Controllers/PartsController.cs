using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclePartsPro.Application.DTOs.Part;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/parts")]
[Authorize(Roles = "Admin")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;

    public PartsController(IPartService partService)
    {
        _partService = partService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var parts = await _partService.GetAllPartsAsync();
        return Ok(parts);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var part = await _partService.GetPartByIdAsync(id);

        if (part == null)
        {
            return NotFound(new { message = $"Part with ID {id} not found." });
        }

        return Ok(part);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartDto dto)
    {
        var result = await _partService.CreatePartAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartDto dto)
    {
        var result = await _partService.UpdatePartAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _partService.DeletePartAsync(id);
        return Ok(new { message = "Part deleted successfully." });
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockParts()
    {
        var result = await _partService.GetLowStockPartsAsync();
        return Ok(result);
    }
}