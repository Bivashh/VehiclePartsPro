using Microsoft.EntityFrameworkCore;
using VehiclePartsPro.Application.DTOs.Sales;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Services;

public class SalesInvoiceService : ISalesInvoiceService
{
    private readonly AppDbContext _db;
    private readonly IEmailService _emailService;

    public SalesInvoiceService(AppDbContext db, IEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task<SalesInvoiceResponseDto> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            throw new InvalidOperationException("Invoice must contain at least one item.");

        var customer = await _db.Customers.FindAsync(dto.CustomerId);

        if (customer == null)
            throw new InvalidOperationException("Customer not found.");

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var invoice = new SalesInvoice
            {
                CustomerId = dto.CustomerId,
                InvoiceDate = DateTime.UtcNow,
                PaymentStatus = dto.PaymentStatus
            };

            decimal subTotal = 0;

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Quantity <= 0)
                    throw new InvalidOperationException("Quantity must be greater than zero.");

                var part = await _db.Parts.FindAsync(itemDto.PartId);

                if (part == null)
                    throw new InvalidOperationException($"Part with ID {itemDto.PartId} not found.");

                if (!part.IsActive)
                    throw new InvalidOperationException($"Part {part.Name} is not active.");

                if (part.StockQuantity < itemDto.Quantity)
                    throw new InvalidOperationException($"Not enough stock for {part.Name}. Available stock: {part.StockQuantity}");

                var lineTotal = part.UnitPrice * itemDto.Quantity;

                var invoiceItem = new SalesInvoiceItem
                {
                    PartId = part.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = part.UnitPrice,
                    LineTotal = lineTotal
                };

                invoice.Items.Add(invoiceItem);

                part.StockQuantity -= itemDto.Quantity;
                part.UpdatedAt = DateTime.UtcNow;

                subTotal += lineTotal;
            }

            var discountAmount = subTotal > 5000 ? subTotal * 0.10m : 0;
            var totalAmount = subTotal - discountAmount;

            invoice.SubTotal = subTotal;
            invoice.DiscountAmount = discountAmount;
            invoice.TotalAmount = totalAmount;

            customer.TotalSpent += totalAmount;

            _db.SalesInvoices.Add(invoice);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            var createdInvoice = await GetSalesInvoiceByIdAsync(invoice.Id)
                ?? throw new InvalidOperationException("Invoice created but could not be loaded.");

            var customerUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == customer.UserId);

            if (customerUser != null && !string.IsNullOrWhiteSpace(customerUser.Email))
            {
                var emailBody = GenerateInvoiceEmailBody(createdInvoice, customerUser.FullName);

                await _emailService.SendEmailAsync(
                    customerUser.Email,
                    $"Invoice #{createdInvoice.InvoiceId} - Vehicle Parts Pro",
                    emailBody
                );
            }

            return createdInvoice;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<SalesInvoiceResponseDto>> GetAllSalesInvoicesAsync()
    {
        return await _db.SalesInvoices
            .Include(si => si.Items)
                .ThenInclude(i => i.Part)
            .OrderByDescending(si => si.InvoiceDate)
            .Select(si => new SalesInvoiceResponseDto
            {
                InvoiceId = si.Id,
                CustomerId = si.CustomerId,
                InvoiceDate = si.InvoiceDate,
                SubTotal = si.SubTotal,
                DiscountAmount = si.DiscountAmount,
                TotalAmount = si.TotalAmount,
                PaymentStatus = si.PaymentStatus,
                Items = si.Items.Select(i => new SalesInvoiceItemResponseDto
                {
                    PartId = i.PartId,
                    PartName = i.Part.Name,
                    PartNumber = i.Part.PartNumber,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<SalesInvoiceResponseDto?> GetSalesInvoiceByIdAsync(int id)
    {
        return await _db.SalesInvoices
            .Include(si => si.Items)
                .ThenInclude(i => i.Part)
            .Where(si => si.Id == id)
            .Select(si => new SalesInvoiceResponseDto
            {
                InvoiceId = si.Id,
                CustomerId = si.CustomerId,
                InvoiceDate = si.InvoiceDate,
                SubTotal = si.SubTotal,
                DiscountAmount = si.DiscountAmount,
                TotalAmount = si.TotalAmount,
                PaymentStatus = si.PaymentStatus,
                Items = si.Items.Select(i => new SalesInvoiceItemResponseDto
                {
                    PartId = i.PartId,
                    PartName = i.Part.Name,
                    PartNumber = i.Part.PartNumber,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    private static string GenerateInvoiceEmailBody(SalesInvoiceResponseDto invoice, string customerName)
    {
        var itemRows = string.Join("", invoice.Items.Select(item => $@"
            <tr>
                <td>{item.PartName}</td>
                <td>{item.PartNumber}</td>
                <td>{item.Quantity}</td>
                <td>Rs. {item.UnitPrice}</td>
                <td>Rs. {item.LineTotal}</td>
            </tr>
        "));

        return $@"
            <h2>Vehicle Parts Pro - Sales Invoice</h2>

            <p>Hello {customerName},</p>
            <p>Thank you for your purchase. Your invoice details are below:</p>

            <h3>Invoice #{invoice.InvoiceId}</h3>
            <p><strong>Date:</strong> {invoice.InvoiceDate:yyyy-MM-dd HH:mm}</p>
            <p><strong>Payment Status:</strong> {invoice.PaymentStatus}</p>

            <table border='1' cellpadding='8' cellspacing='0'>
                <thead>
                    <tr>
                        <th>Part</th>
                        <th>Part Number</th>
                        <th>Quantity</th>
                        <th>Unit Price</th>
                        <th>Line Total</th>
                    </tr>
                </thead>
                <tbody>
                    {itemRows}
                </tbody>
            </table>

            <br/>

            <p><strong>Subtotal:</strong> Rs. {invoice.SubTotal}</p>
            <p><strong>Discount:</strong> Rs. {invoice.DiscountAmount}</p>
            <p><strong>Total Amount:</strong> Rs. {invoice.TotalAmount}</p>

            <p>Regards,<br/>Vehicle Parts Pro</p>
        ";
    }
}