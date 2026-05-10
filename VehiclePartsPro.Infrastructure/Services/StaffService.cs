using Microsoft.EntityFrameworkCore;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Services;

public class StaffService : IStaffService
{
    private readonly AppDbContext _db;

    public StaffService(AppDbContext db)
    {
        _db = db;
    }

    // =========================================
    // CREATE STAFF
    // =========================================
    public async Task CreateStaffAsync(Staff staff)
    {
        _db.Staffs.Add(staff);

        await _db.SaveChangesAsync();
    }

    // =========================================
    // GET ALL STAFF (ADMIN)
    // =========================================
    public async Task<List<Staff>> GetAllStaffAsync()
    {
        return await _db.Staffs.ToListAsync();
    }

    // =========================================
    // GET STAFF BY ID (ADMIN)
    // =========================================
    public async Task<Staff?> GetStaffByIdAsync(int id)
    {
        return await _db.Staffs
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // =========================================
    // GET STAFF BY USER ID (SELF PROFILE)
    // =========================================
    public async Task<Staff?> GetStaffByUserIdAsync(string userId)
    {
        return await _db.Staffs
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    // =========================================
    // UPDATE STAFF
    // =========================================
    public async Task UpdateStaffAsync(Staff staff)
    {
        _db.Staffs.Update(staff);

        await _db.SaveChangesAsync();
    }

    // =========================================
    // DELETE STAFF
    // =========================================
    public async Task DeleteStaffAsync(Staff staff)
    {
        _db.Staffs.Remove(staff);

        await _db.SaveChangesAsync();
    }
}