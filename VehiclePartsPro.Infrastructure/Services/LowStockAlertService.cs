using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehiclePartsPro.Application.DTOs.LowStockAlert;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Services;

public class LowStockAlertService : ILowStockAlertService
{
    private readonly AppDbContext _db;
    private readonly ILogger<LowStockAlertService> _logger;

    public LowStockAlertService(AppDbContext db, ILogger<LowStockAlertService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<LowStockAlertDto>> GetAllAlertsAsync()
    {
        return await _db.LowStockAlerts
            .AsNoTracking()
            .Include(a => a.Part)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => MapToDto(a))
            .ToListAsync();
    }

    public async Task<List<LowStockAlertDto>> GetActiveAlertsAsync()
    {
        return await _db.LowStockAlerts
            .AsNoTracking()
            .Include(a => a.Part)
            .Where(a => !a.IsResolved)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => MapToDto(a))
            .ToListAsync();
    }

    public async Task<List<LowStockAlertDto>> GenerateLowStockAlertsAsync()
    {
        var lowStockParts = await _db.Parts
            .Where(p => p.IsActive && p.StockQuantity < p.LowStockThreshold)
            .ToListAsync();

        foreach (var part in lowStockParts)
        {
            var activeAlertExists = await _db.LowStockAlerts
                .AnyAsync(a => a.PartId == part.Id && !a.IsResolved);

            if (activeAlertExists)
            {
                continue;
            }

            var alert = new LowStockAlert
            {
                PartId = part.Id,
                CurrentStock = part.StockQuantity,
                Threshold = part.LowStockThreshold,
                Message = $"{part.Name} stock is low. Current stock: {part.StockQuantity}, threshold: {part.LowStockThreshold}.",
                IsResolved = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.LowStockAlerts.Add(alert);
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation("Low stock alert generation completed.");

        return await GetActiveAlertsAsync();
    }

    public async Task<LowStockAlertDto> ResolveAlertAsync(int id)
    {
        var alert = await _db.LowStockAlerts
            .Include(a => a.Part)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alert == null)
        {
            throw new KeyNotFoundException($"Low stock alert with ID {id} not found.");
        }

        if (!alert.IsResolved)
        {
            alert.IsResolved = true;
            alert.ResolvedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Low stock alert resolved: {AlertId}", id);
        }

        return MapToDto(alert);
    }

    private static LowStockAlertDto MapToDto(LowStockAlert alert)
    {
        return new LowStockAlertDto
        {
            Id = alert.Id,
            PartId = alert.PartId,
            PartName = alert.Part?.Name ?? string.Empty,
            PartNumber = alert.Part?.PartNumber ?? string.Empty,
            CurrentStock = alert.CurrentStock,
            Threshold = alert.Threshold,
            Message = alert.Message,
            IsResolved = alert.IsResolved,
            CreatedAt = alert.CreatedAt,
            ResolvedAt = alert.ResolvedAt
        };
    }
}