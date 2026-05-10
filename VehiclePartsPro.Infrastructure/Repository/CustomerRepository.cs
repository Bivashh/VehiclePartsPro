using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Repository;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Customer customer)
    {
        await _db.Customers.AddAsync(customer);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}