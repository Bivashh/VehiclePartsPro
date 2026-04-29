using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehiclePartsPro.Infrastructure.Data;
using VehiclePartsPro.Application.DTOs.Staff;
using VehiclePartsPro.Application.DTOs.Customer;
using VehiclePartsPro.Application.Interfaces;

namespace VehiclePartsPro.Application.Services;

// ── Feature 2: Admin manages staff ────────────────────────────────────────────

public class StaffService : IStaffService
{
    private readonly AppDbContext _db;
    private readonly ILogger<StaffService> _logger;

    public StaffService(AppDbContext db, ILogger<StaffService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<StaffDto>> GetAllStaffAsync()
    {
        return await _db.Staff
            .Include(s => s.User)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<StaffDto?> GetStaffByIdAsync(int id)
    {
        var staff = await _db.Staff
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);

        return staff is null ? null : MapToDto(staff);
    }

    public async Task<StaffDto> UpdateStaffAsync(int id, UpdateStaffDto dto)
    {
        var staff = await _db.Staff
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new KeyNotFoundException($"Staff with ID {id} not found.");

        staff.User.FullName = dto.FullName;
        staff.Phone = dto.Phone;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Staff ID {Id} updated", id);

        return MapToDto(staff);
    }

    public async Task DeleteStaffAsync(int id)
    {
        var staff = await _db.Staff
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new KeyNotFoundException($"Staff with ID {id} not found.");

        // Soft delete: lock the account rather than removing data
        staff.User.LockoutEnd = DateTimeOffset.MaxValue;
        staff.User.LockoutEnabled = true;

        await _db.SaveChangesAsync();
        _logger.LogWarning("Staff ID {Id} deactivated (locked)", id);
    }

    private static StaffDto MapToDto(Domain.Entities.Staff s) => new()
    {
        Id = s.Id,
        UserId = s.UserId,
        FullName = s.User.FullName,
        Email = s.User.Email ?? string.Empty,
        Phone = s.Phone,
        EmployeeCode = s.EmployeeCode,
        HiredAt = s.HiredAt
    };
}

// ── Features 8, 10, 12: View, search, update customers ───────────────────────

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(AppDbContext db, ILogger<CustomerService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<CustomerDto>> GetAllCustomersAsync()
    {
        return await _db.Customers
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .Select(c => MapToDto(c))
            .ToListAsync();
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var customer = await _db.Customers
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Id == id);

        return customer is null ? null : MapToDto(customer);
    }

    public async Task<CustomerDto?> GetCustomerByUserIdAsync(string userId)
    {
        var customer = await _db.Customers
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        return customer is null ? null : MapToDto(customer);
    }

    public async Task<CustomerDto> UpdateCustomerProfileAsync(string userId, UpdateCustomerProfileDto dto)
    {
        var customer = await _db.Customers
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.UserId == userId)
            ?? throw new KeyNotFoundException("Customer profile not found.");

        customer.User.FullName = dto.FullName;
        customer.Phone = dto.Phone;
        customer.Address = dto.Address;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Customer profile updated for UserId {UserId}", userId);

        return MapToDto(customer);
    }

    // Feature 10: Search by name, phone, email, vehicle plate, or ID
    public async Task<List<CustomerDto>> SearchCustomersAsync(CustomerSearchDto search)
    {
        var query = _db.Customers
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .AsQueryable();

        if (search.CustomerId.HasValue)
            query = query.Where(c => c.Id == search.CustomerId.Value);

        if (!string.IsNullOrWhiteSpace(search.Name))
            query = query.Where(c => c.User.FullName.ToLower().Contains(search.Name.ToLower()));

        if (!string.IsNullOrWhiteSpace(search.Phone))
            query = query.Where(c => c.Phone.Contains(search.Phone));

        if (!string.IsNullOrWhiteSpace(search.Email))
            query = query.Where(c => c.User.Email!.ToLower().Contains(search.Email.ToLower()));

        if (!string.IsNullOrWhiteSpace(search.VehiclePlate))
            query = query.Where(c => c.Vehicles.Any(v =>
                v.PlateNumber.ToLower().Contains(search.VehiclePlate.ToLower())));

        return await query.Select(c => MapToDto(c)).ToListAsync();
    }

    private static CustomerDto MapToDto(Domain.Entities.Customer c) => new()
    {
        Id = c.Id,
        UserId = c.UserId,
        FullName = c.User.FullName,
        Email = c.User.Email ?? string.Empty,
        Phone = c.Phone,
        Address = c.Address,
        CreditBalance = c.CreditBalance,
        TotalSpent = c.TotalSpent,
        CreditDueDate = c.CreditDueDate,
        Vehicles = c.Vehicles.Select(v => new VehicleDto
        {
            Id = v.Id,
            PlateNumber = v.PlateNumber,
            Make = v.Make,
            Model = v.Model,
            Year = v.Year,
            Notes = v.Notes
        }).ToList()
    };
}