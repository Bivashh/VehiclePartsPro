using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehiclePartsPro.Application.DTOs.Customer;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;
using VehiclePartsPro.Infrastructure.Identity;

namespace VehiclePartsPro.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;

    public CustomerService(AppDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
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

    public async Task<CustomerDto> RegisterCustomerWithVehicleAsync(RegisterCustomerWithVehicleDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
            throw new InvalidOperationException("Email already exists.");

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var user = new User
            {
                FullName = dto.FullName,
                UserName = dto.Email,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            var createUserResult = await _userManager.CreateAsync(user, dto.Password);

            if (!createUserResult.Succeeded)
            {
                var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            var customer = new Customer
            {
                UserId = user.Id,
                Phone = dto.Phone,
                Address = dto.Address,
                CreditBalance = 0,
                TotalSpent = 0,
                CreditDueDate = null
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            var vehicle = new Vehicle
            {
                CustomerId = customer.Id,
                PlateNumber = dto.PlateNumber,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                Notes = dto.Notes
            };

            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return new CustomerDto
            {
                Id = customer.Id,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Phone = customer.Phone,
                Address = customer.Address,
                CreditBalance = customer.CreditBalance,
                TotalSpent = customer.TotalSpent,
                CreditDueDate = customer.CreditDueDate,
                Vehicles = new List<VehicleDto>
                {
                    new VehicleDto
                    {
                        Id = vehicle.Id,
                        PlateNumber = vehicle.PlateNumber,
                        Make = vehicle.Make,
                        Model = vehicle.Model,
                        Year = vehicle.Year,
                        Notes = vehicle.Notes
                    }
                }
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}