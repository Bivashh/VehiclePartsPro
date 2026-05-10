using VehiclePartsPro.Domain.Entities;

namespace VehiclePartsPro.Application.Interfaces;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer);
    Task SaveChangesAsync();
}