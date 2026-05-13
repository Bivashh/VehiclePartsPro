using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclePartsPro.Application.DTOs.Sales;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/sales-invoices")]
[Authorize(Roles = "Admin,Staff")]
public class SalesInvoicesController : ControllerBase
{
    private readonly ISalesInvoiceService _salesInvoiceService;

    public SalesInvoicesController(ISalesInvoiceService salesInvoiceService)
    {
        _salesInvoiceService = salesInvoiceService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSalesInvoice(CreateSalesInvoiceDto dto)
    {
        var result = await _salesInvoiceService.CreateSalesInvoiceAsync(dto);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSalesInvoices()
    {
        var result = await _salesInvoiceService.GetAllSalesInvoicesAsync();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSalesInvoiceById(int id)
    {
        var result = await _salesInvoiceService.GetSalesInvoiceByIdAsync(id);

        if (result == null)
            return NotFound("Sales invoice not found.");

        return Ok(result);
    }
}