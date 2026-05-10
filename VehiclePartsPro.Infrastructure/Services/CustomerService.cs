using Microsoft.EntityFrameworkCore;
using VehiclePartsPro.Application.DTOs.Customer;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;

    public CustomerService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Customer?> GetCustomerByUserIdAsync(string userId)
    {
        return await _db.Customers
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task CreateCustomerAsync(Customer customer)
    {
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
        _db.Customers.Update(customer);
        await _db.SaveChangesAsync();
    }

    public async Task<List<VehicleDto>> GetVehiclesAsync(string userId)
    {
        return await _db.Vehicles
            .Where(v => v.Customer.UserId == userId)
            .Select(v => new VehicleDto
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Notes = v.Notes
            })
            .ToListAsync();
    }

    public async Task AddVehicleAsync(Vehicle vehicle)
    {
        _db.Vehicles.Add(vehicle);
        await _db.SaveChangesAsync();
    }

    public async Task<Vehicle?> GetVehicleByIdAsync(int id)
    {
        return await _db.Vehicles.FindAsync(id);
    }

    public async Task UpdateVehicleAsync(Vehicle vehicle)
    {
        _db.Vehicles.Update(vehicle);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteVehicleAsync(Vehicle vehicle)
    {
        _db.Vehicles.Remove(vehicle);
        await _db.SaveChangesAsync();
    }
}