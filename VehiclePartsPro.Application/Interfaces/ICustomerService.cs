using VehiclePartsPro.Application.DTOs.Customer;
using VehiclePartsPro.Domain.Entities;

namespace VehiclePartsPro.Application.Interfaces;

public interface ICustomerService
{
    Task<Customer?> GetCustomerByUserIdAsync(string userId);

    Task CreateCustomerAsync(Customer customer);

    Task UpdateCustomerAsync(Customer customer);

    Task<List<VehicleDto>> GetVehiclesAsync(string userId);

    Task AddVehicleAsync(Vehicle vehicle);

    Task<Vehicle?> GetVehicleByIdAsync(int id);

    Task UpdateVehicleAsync(Vehicle vehicle);

    Task DeleteVehicleAsync(Vehicle vehicle);

    Task<CustomerDto> RegisterCustomerWithVehicleAsync(RegisterCustomerWithVehicleDto dto);

    Task<List<CustomerReportDto>> GetCustomerReportsAsync();

    Task<List<CustomerHistoryDto>> GetCustomerHistoryAsync(int customerId);

    Task<List<CustomerReportDto>> SearchCustomersAsync(CustomerSearchDto dto);

}