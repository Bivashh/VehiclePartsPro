using VehiclePartsPro.Application.DTOs.Staff;
using VehiclePartsPro.Domain.Entities;

namespace VehiclePartsPro.Application.Interfaces;

public interface IStaffService
{
    Task CreateStaffAsync(Staff staff);

    Task<List<StaffDto>> GetAllStaffAsync();

    Task<Staff?> GetStaffByIdAsync(int id);

    Task<Staff?> GetStaffByUserIdAsync(string userId);

    Task UpdateStaffAsync(Staff staff);

    Task DeleteStaffAsync(Staff staff);
}