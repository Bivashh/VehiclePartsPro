using Microsoft.EntityFrameworkCore;
using VehiclePartsPro.Application.DTOs.PartRequest;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Services;

public class PartRequestService : IPartRequestService
{
    private readonly AppDbContext _db;

    public PartRequestService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PartRequestDto> CreatePartRequestAsync(string userId, CreatePartRequestDto dto)
    {
        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null)
            throw new InvalidOperationException("Customer profile not found.");

        var request = new PartRequest
        {
            CustomerId = customer.Id,
            PartName = dto.PartName,
            Description = dto.Description,
            Status = "Pending",
            RequestedAt = DateTime.UtcNow
        };

        _db.PartRequests.Add(request);
        await _db.SaveChangesAsync();

        var created = await _db.PartRequests
            .Include(r => r.Customer)
            .FirstAsync(r => r.Id == request.Id);

        return await MapToDtoAsync(created);
    }

    public async Task<List<PartRequestDto>> GetMyPartRequestsAsync(string userId)
    {
        var requests = await _db.PartRequests
            .Include(r => r.Customer)
            .Where(r => r.Customer.UserId == userId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();

        var result = new List<PartRequestDto>();

        foreach (var request in requests)
        {
            result.Add(await MapToDtoAsync(request));
        }

        return result;
    }

    public async Task<List<PartRequestDto>> GetAllPartRequestsAsync()
    {
        var requests = await _db.PartRequests
            .Include(r => r.Customer)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();

        var result = new List<PartRequestDto>();

        foreach (var request in requests)
        {
            result.Add(await MapToDtoAsync(request));
        }

        return result;
    }

    public async Task<PartRequestDto?> UpdateStatusAsync(int id, UpdatePartRequestStatusDto dto)
    {
        var request = await _db.PartRequests
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
            return null;

        var allowedStatuses = new[] { "Pending", "Ordered", "Completed", "Rejected" };

        if (!allowedStatuses.Contains(dto.Status))
            throw new InvalidOperationException("Invalid request status.");

        request.Status = dto.Status;
        request.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return await MapToDtoAsync(request);
    }

    private async Task<PartRequestDto> MapToDtoAsync(PartRequest request)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.Customer.UserId);

        return new PartRequestDto
        {
            Id = request.Id,
            CustomerId = request.CustomerId,
            CustomerName = user?.FullName ?? "",
            CustomerEmail = user?.Email ?? "",
            PartName = request.PartName,
            Description = request.Description,
            Status = request.Status,
            RequestedAt = request.RequestedAt,
            UpdatedAt = request.UpdatedAt
        };
    }
}