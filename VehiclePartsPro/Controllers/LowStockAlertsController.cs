using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/low-stock-alerts")]
[Authorize(Roles = "Admin")]
public class LowStockAlertsController : ControllerBase
{
    private readonly ILowStockAlertService _lowStockAlertService;

    public LowStockAlertsController(ILowStockAlertService lowStockAlertService)
    {
        _lowStockAlertService = lowStockAlertService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var alerts = await _lowStockAlertService.GetAllAlertsAsync();
        return Ok(alerts);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var alerts = await _lowStockAlertService.GetActiveAlertsAsync();
        return Ok(alerts);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate()
    {
        var alerts = await _lowStockAlertService.GenerateLowStockAlertsAsync();
        return Ok(alerts);
    }

    [HttpPut("{id:int}/resolve")]
    public async Task<IActionResult> Resolve(int id)
    {
        var result = await _lowStockAlertService.ResolveAlertAsync(id);
        return Ok(result);
    }
}