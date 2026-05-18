using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // =========================================
    // FINANCIAL SUMMARY
    // =========================================
    [HttpGet("financial-summary")]
    public async Task<IActionResult> GetFinancialSummary()
    {
        var result = await _reportService.GetFinancialSummaryAsync();

        return Ok(result);
    }

    // =========================================
    // MONTHLY SALES REPORT
    // =========================================
    [HttpGet("monthly-sales")]
    public async Task<IActionResult> GetMonthlySales()
    {
        var result = await _reportService.GetMonthlySalesReportAsync();

        return Ok(result);
    }

    // =========================================
    // TOP SELLING PARTS
    // =========================================
    [HttpGet("top-selling-parts")]
    public async Task<IActionResult> GetTopSellingParts()
    {
        var result = await _reportService.GetTopSellingPartsAsync();

        return Ok(result);
    }
}