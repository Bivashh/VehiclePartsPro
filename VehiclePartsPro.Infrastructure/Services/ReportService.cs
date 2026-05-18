using Microsoft.EntityFrameworkCore;
using VehiclePartsPro.Application.DTOs;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;

    public ReportService(AppDbContext db)
    {
        _db = db;
    }

    // =========================================
    // FINANCIAL SUMMARY
    // =========================================
    public async Task<FinancialSummaryDto> GetFinancialSummaryAsync()
    {
        var totalSalesRevenue = await _db.SalesInvoices
            .SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

        var totalPurchaseCost = await _db.PurchaseInvoices
            .SumAsync(p => (decimal?)p.TotalAmount) ?? 0;

        var totalSalesInvoices = await _db.SalesInvoices.CountAsync();

        var totalPurchaseInvoices = await _db.PurchaseInvoices.CountAsync();

        var totalPartsSold = await _db.SalesInvoiceItems
            .SumAsync(i => (int?)i.Quantity) ?? 0;

        var totalPartsPurchased = await _db.PurchaseInvoiceItems
            .SumAsync(i => (int?)i.Quantity) ?? 0;

        return new FinancialSummaryDto
        {
            TotalSalesRevenue = totalSalesRevenue,
            TotalPurchaseCost = totalPurchaseCost,
            EstimatedProfit = totalSalesRevenue - totalPurchaseCost,
            TotalSalesInvoices = totalSalesInvoices,
            TotalPurchaseInvoices = totalPurchaseInvoices,
            TotalPartsSold = totalPartsSold,
            TotalPartsPurchased = totalPartsPurchased
        };
    }

    // =========================================
    // MONTHLY SALES REPORT
    // =========================================
    public async Task<List<MonthlySalesDto>> GetMonthlySalesReportAsync()
    {
        return await _db.SalesInvoices
            .GroupBy(s => new
            {
                s.InvoiceDate.Year,
                s.InvoiceDate.Month
            })
            .Select(g => new MonthlySalesDto
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                SalesRevenue = g.Sum(x => x.TotalAmount)
            })
            .OrderBy(x => x.Month)
            .ToListAsync();
    }

    // =========================================
    // TOP SELLING PARTS
    // =========================================
    public async Task<List<TopSellingPartDto>> GetTopSellingPartsAsync()
    {
        return await _db.SalesInvoiceItems
            .Include(i => i.Part)
            .GroupBy(i => new
            {
                i.PartId,
                i.Part.Name,
                i.Part.PartNumber
            })
            .Select(g => new TopSellingPartDto
            {
                PartId = g.Key.PartId,
                PartName = g.Key.Name,
                PartNumber = g.Key.PartNumber,
                QuantitySold = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(10)
            .ToListAsync();
    }
}