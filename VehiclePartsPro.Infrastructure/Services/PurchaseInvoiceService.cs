using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehiclePartsPro.Application.DTOs.PurchaseInvoice;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Services;

public class PurchaseInvoiceService : IPurchaseInvoiceService
{
    private readonly AppDbContext _db;
    private readonly ILogger<PurchaseInvoiceService> _logger;

    public PurchaseInvoiceService(AppDbContext db, ILogger<PurchaseInvoiceService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<PurchaseInvoiceDto>> GetAllPurchaseInvoicesAsync()
    {
        return await _db.PurchaseInvoices
            .AsNoTracking()
            .Include(pi => pi.Vendor)
            .Include(pi => pi.Items)
                .ThenInclude(item => item.Part)
            .OrderByDescending(pi => pi.PurchaseDate)
            .Select(pi => MapToDto(pi))
            .ToListAsync();
    }

    public async Task<PurchaseInvoiceDto?> GetPurchaseInvoiceByIdAsync(int id)
    {
        var invoice = await _db.PurchaseInvoices
            .AsNoTracking()
            .Include(pi => pi.Vendor)
            .Include(pi => pi.Items)
                .ThenInclude(item => item.Part)
            .FirstOrDefaultAsync(pi => pi.Id == id);

        return invoice == null ? null : MapToDto(invoice);
    }

    public async Task<PurchaseInvoiceDto> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceDto dto)
    {
        var invoiceNumber = dto.InvoiceNumber.Trim();

        var duplicateInvoice = await _db.PurchaseInvoices
            .AnyAsync(pi => pi.InvoiceNumber.ToLower() == invoiceNumber.ToLower());

        if (duplicateInvoice)
        {
            throw new InvalidOperationException($"Invoice number '{invoiceNumber}' already exists.");
        }

        var vendor = await _db.Vendors
            .FirstOrDefaultAsync(v => v.Id == dto.VendorId && v.IsActive);

        if (vendor == null)
        {
            throw new KeyNotFoundException($"Vendor with ID {dto.VendorId} not found.");
        }

        if (dto.Items == null || dto.Items.Count == 0)
        {
            throw new InvalidOperationException("At least one invoice item is required.");
        }

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var invoice = new PurchaseInvoice
            {
                InvoiceNumber = invoiceNumber,
                VendorId = dto.VendorId,
                PurchaseDate = dto.PurchaseDate?.ToUniversalTime() ?? DateTime.UtcNow,
                Notes = dto.Notes?.Trim(),
                CreatedAt = DateTime.UtcNow,
                Items = new List<PurchaseInvoiceItem>()
            };

            decimal totalAmount = 0;

            foreach (var itemDto in dto.Items)
            {
                var part = await _db.Parts
                    .FirstOrDefaultAsync(p => p.Id == itemDto.PartId && p.IsActive);

                if (part == null)
                {
                    throw new KeyNotFoundException($"Part with ID {itemDto.PartId} not found.");
                }

                if (itemDto.Quantity <= 0)
                {
                    throw new InvalidOperationException("Quantity must be greater than 0.");
                }

                if (itemDto.UnitCost <= 0)
                {
                    throw new InvalidOperationException("Unit cost must be greater than 0.");
                }

                var lineTotal = itemDto.Quantity * itemDto.UnitCost;

                var invoiceItem = new PurchaseInvoiceItem
                {
                    PartId = itemDto.PartId,
                    Quantity = itemDto.Quantity,
                    UnitCost = itemDto.UnitCost,
                    LineTotal = lineTotal
                };

                invoice.Items.Add(invoiceItem);
                totalAmount += lineTotal;

                // Important stock update logic:
                // Purchase invoice increases part stock.
                part.StockQuantity += itemDto.Quantity;
                part.UpdatedAt = DateTime.UtcNow;
            }

            invoice.TotalAmount = totalAmount;

            _db.PurchaseInvoices.Add(invoice);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            _logger.LogInformation(
                "Purchase invoice created: {InvoiceNumber}, Total: {TotalAmount}",
                invoice.InvoiceNumber,
                invoice.TotalAmount
            );

            var createdInvoice = await _db.PurchaseInvoices
                .AsNoTracking()
                .Include(pi => pi.Vendor)
                .Include(pi => pi.Items)
                    .ThenInclude(item => item.Part)
                .FirstAsync(pi => pi.Id == invoice.Id);

            return MapToDto(createdInvoice);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static PurchaseInvoiceDto MapToDto(PurchaseInvoice invoice)
    {
        return new PurchaseInvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            VendorId = invoice.VendorId,
            VendorName = invoice.Vendor?.Name ?? string.Empty,
            PurchaseDate = invoice.PurchaseDate,
            TotalAmount = invoice.TotalAmount,
            Notes = invoice.Notes,
            Items = invoice.Items.Select(item => new PurchaseInvoiceItemDto
            {
                Id = item.Id,
                PartId = item.PartId,
                PartName = item.Part?.Name ?? string.Empty,
                PartNumber = item.Part?.PartNumber ?? string.Empty,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost,
                LineTotal = item.LineTotal
            }).ToList()
        };
    }
}
