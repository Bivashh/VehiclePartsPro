using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclePartsPro.Application.DTOs.PurchaseInvoice;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/purchase-invoices")]
[Authorize(Roles = "Admin")]
public class PurchaseInvoicesController : ControllerBase
{
    private readonly IPurchaseInvoiceService _purchaseInvoiceService;

    public PurchaseInvoicesController(IPurchaseInvoiceService purchaseInvoiceService)
    {
        _purchaseInvoiceService = purchaseInvoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var invoices = await _purchaseInvoiceService.GetAllPurchaseInvoicesAsync();
        return Ok(invoices);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await _purchaseInvoiceService.GetPurchaseInvoiceByIdAsync(id);

        if (invoice == null)
        {
            return NotFound(new { message = $"Purchase invoice with ID {id} not found." });
        }

        return Ok(invoice);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseInvoiceDto dto)
    {
        var result = await _purchaseInvoiceService.CreatePurchaseInvoiceAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}