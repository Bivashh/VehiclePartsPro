using VehiclePartsPro.Domain.Entities;

namespace VehiclePartsPro.Application.Interfaces;

public interface IStaffService
{
    Task CreateStaffAsync(Staff staff);

    Task<List<Staff>> GetAllStaffAsync();

    Task<Staff?> GetStaffByIdAsync(int id);

    Task<Staff?> GetStaffByUserIdAsync(string userId);

    Task UpdateStaffAsync(Staff staff);

    Task DeleteStaffAsync(Staff staff);
}