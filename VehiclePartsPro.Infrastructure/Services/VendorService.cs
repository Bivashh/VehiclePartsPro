using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehiclePartsPro.Application.DTOs.Vendor;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Services;

public class VendorService : IVendorService
{
    private readonly AppDbContext _db;
    private readonly ILogger<VendorService> _logger;

    public VendorService(AppDbContext db, ILogger<VendorService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<VendorDto>> GetAllVendorsAsync()
    {
        return await _db.Vendors
            .AsNoTracking()
            .Where(v => v.IsActive)
            .OrderBy(v => v.Name)
            .Select(v => MapToDto(v))
            .ToListAsync();
    }

    public async Task<VendorDto?> GetVendorByIdAsync(int id)
    {
        var vendor = await _db.Vendors
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        return vendor == null ? null : MapToDto(vendor);
    }

    public async Task<VendorDto> CreateVendorAsync(CreateVendorDto dto)
    {
        var email = NormalizeEmail(dto.Email);

        if (!string.IsNullOrWhiteSpace(email))
        {
            var duplicateEmailExists = await _db.Vendors
                .AnyAsync(v =>
                    v.Email != null &&
                    v.Email.ToLower() == email);

            if (duplicateEmailExists)
            {
                throw new InvalidOperationException($"Vendor email '{dto.Email}' already exists.");
            }
        }

        var vendor = new Vendor
        {
            Name = dto.Name.Trim(),
            ContactPerson = dto.ContactPerson?.Trim(),
            PhoneNumber = dto.PhoneNumber?.Trim(),
            Email = email,
            Address = dto.Address?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Vendors.Add(vendor);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Vendor created: {VendorName}", vendor.Name);

        return MapToDto(vendor);
    }

    public async Task<VendorDto> UpdateVendorAsync(int id, UpdateVendorDto dto)
    {
        var vendor = await _db.Vendors
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        if (vendor == null)
        {
            throw new KeyNotFoundException($"Vendor with ID {id} not found.");
        }

        var email = NormalizeEmail(dto.Email);

        if (!string.IsNullOrWhiteSpace(email))
        {
            var duplicateEmailExists = await _db.Vendors
                .AnyAsync(v =>
                    v.Id != id &&
                    v.Email != null &&
                    v.Email.ToLower() == email);

            if (duplicateEmailExists)
            {
                throw new InvalidOperationException($"Vendor email '{dto.Email}' already exists.");
            }
        }

        vendor.Name = dto.Name.Trim();
        vendor.ContactPerson = dto.ContactPerson?.Trim();
        vendor.PhoneNumber = dto.PhoneNumber?.Trim();
        vendor.Email = email;
        vendor.Address = dto.Address?.Trim();
        vendor.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Vendor updated: {VendorId}", id);

        return MapToDto(vendor);
    }

    public async Task DeleteVendorAsync(int id)
    {
        var vendor = await _db.Vendors
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        if (vendor == null)
        {
            throw new KeyNotFoundException($"Vendor with ID {id} not found.");
        }

        vendor.IsActive = false;
        vendor.UpdatedAt = DateTime.UtcNow;

        // Important:
        // Because Email has a unique index in the database,
        // clearing the email allows the same email to be reused
        // if a vendor is deleted and created again later.
        vendor.Email = null;

        await _db.SaveChangesAsync();

        _logger.LogWarning("Vendor deleted: {VendorId}", id);
    }

    private static string? NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return email.Trim().ToLower();
    }

    private static VendorDto MapToDto(Vendor vendor)
    {
        return new VendorDto
        {
            Id = vendor.Id,
            Name = vendor.Name,
            ContactPerson = vendor.ContactPerson,
            PhoneNumber = vendor.PhoneNumber,
            Email = vendor.Email,
            Address = vendor.Address,
            IsActive = vendor.IsActive
        };
    }
}