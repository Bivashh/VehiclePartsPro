using VehiclePartsPro.Application.DTOs.Customer;
using VehiclePartsPro.Application.DTOs.Staff;

namespace VehiclePartsPro.Application.Interfaces;

public interface IStaffService
{
    Task<List<StaffDto>> GetAllStaffAsync();
    Task<StaffDto?> GetStaffByIdAsync(int id);
    Task<StaffDto> UpdateStaffAsync(int id, UpdateStaffDto dto);
    Task DeleteStaffAsync(int id);
}

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto?> GetCustomerByIdAsync(int id);
    Task<CustomerDto?> GetCustomerByUserIdAsync(string userId);
    Task<CustomerDto> UpdateCustomerProfileAsync(string userId, UpdateCustomerProfileDto dto);
    Task<List<CustomerDto>> SearchCustomersAsync(CustomerSearchDto searchDto);
}