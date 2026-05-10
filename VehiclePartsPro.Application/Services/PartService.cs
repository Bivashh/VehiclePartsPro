using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehiclePartsPro.Application.DTOs.Part;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Application.Services;

public class PartService : IPartService
{
    private readonly AppDbContext _db;
    private readonly ILogger<PartService> _logger;

    public PartService(AppDbContext db, ILogger<PartService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<PartDto>> GetAllPartsAsync()
    {
        return await _db.Parts
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<PartDto?> GetPartByIdAsync(int id)
    {
        var part = await _db.Parts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        return part is null ? null : MapToDto(part);
    }

    public async Task<PartDto> CreatePartAsync(CreatePartDto dto)
    {
        var partNumber = dto.PartNumber.Trim();

        var duplicateExists = await _db.Parts
            .AnyAsync(p => p.PartNumber.ToLower() == partNumber.ToLower() && p.IsActive);

        if (duplicateExists)
            throw new InvalidOperationException($"Part number '{partNumber}' already exists.");

        var part = new Part
        {
            Name = dto.Name.Trim(),
            PartNumber = partNumber,
            Category = dto.Category.Trim(),
            Description = dto.Description,
            UnitPrice = dto.UnitPrice,
            StockQuantity = dto.StockQuantity,
            LowStockThreshold = dto.LowStockThreshold,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.Parts.Add(part);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Part created: {PartNumber}", part.PartNumber);

        return MapToDto(part);
    }

    public async Task<PartDto> UpdatePartAsync(int id, UpdatePartDto dto)
    {
        var part = await _db.Parts
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive)
            ?? throw new KeyNotFoundException($"Part with ID {id} not found.");

        var partNumber = dto.PartNumber.Trim();

        var duplicateExists = await _db.Parts
            .AnyAsync(p =>
                p.Id != id &&
                p.PartNumber.ToLower() == partNumber.ToLower() &&
                p.IsActive);

        if (duplicateExists)
            throw new InvalidOperationException($"Part number '{partNumber}' already exists.");

        part.Name = dto.Name.Trim();
        part.PartNumber = partNumber;
        part.Category = dto.Category.Trim();
        part.Description = dto.Description;
        part.UnitPrice = dto.UnitPrice;
        part.StockQuantity = dto.StockQuantity;
        part.LowStockThreshold = dto.LowStockThreshold;
        part.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Part updated: {PartId}", id);

        return MapToDto(part);
    }

    public async Task DeletePartAsync(int id)
    {
        var part = await _db.Parts
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive)
            ?? throw new KeyNotFoundException($"Part with ID {id} not found.");

        part.IsActive = false;
        part.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogWarning("Part deleted: {PartId}", id);
    }

    public async Task<List<PartDto>> GetLowStockPartsAsync()
    {
        return await _db.Parts
            .AsNoTracking()
            .Where(p => p.IsActive && p.StockQuantity < p.LowStockThreshold)
            .OrderBy(p => p.StockQuantity)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    private static PartDto MapToDto(Part part)
    {
        return new PartDto
        {
            Id = part.Id,
            Name = part.Name,
            PartNumber = part.PartNumber,
            Category = part.Category,
            Description = part.Description,
            UnitPrice = part.UnitPrice,
            StockQuantity = part.StockQuantity,
            LowStockThreshold = part.LowStockThreshold,
            IsLowStock = part.StockQuantity < part.LowStockThreshold,
            IsActive = part.IsActive
        };
    }
}