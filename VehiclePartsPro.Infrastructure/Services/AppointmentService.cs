using Microsoft.EntityFrameworkCore;
using VehiclePartsPro.Application.DTOs.Appointment;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _db;

    public AppointmentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AppointmentDto> CreateAppointmentAsync(
        string userId,
        CreateAppointmentDto dto)
    {
        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null)
            throw new Exception("Customer not found.");

        var vehicle = await _db.Vehicles
            .FirstOrDefaultAsync(v =>
                v.Id == dto.VehicleId &&
                v.CustomerId == customer.Id);

        if (vehicle == null)
            throw new Exception("Vehicle not found.");

        var appointment = new Appointment
        {
            CustomerId = customer.Id,
            VehicleId = vehicle.Id,
            ServiceType = dto.ServiceType,
            Notes = dto.Notes,
            AppointmentDate = dto.AppointmentDate,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _db.Appointments.Add(appointment);

        await _db.SaveChangesAsync();

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == customer.UserId);

        return new AppointmentDto
        {
            Id = appointment.Id,
            CustomerId = customer.Id,
            CustomerName = user?.FullName ?? "",
            VehicleId = vehicle.Id,
            VehiclePlate = vehicle.PlateNumber,
            VehicleName = $"{vehicle.Make} {vehicle.Model}",
            ServiceType = appointment.ServiceType,
            Notes = appointment.Notes,
            AppointmentDate = appointment.AppointmentDate,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt
        };
    }

    public async Task<List<AppointmentDto>> GetMyAppointmentsAsync(string userId)
    {
        return await _db.Appointments
            .Include(a => a.Vehicle)
            .Include(a => a.Customer)
            .Where(a => a.Customer.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                VehicleId = a.VehicleId,
                VehiclePlate = a.Vehicle.PlateNumber,
                VehicleName = a.Vehicle.Make + " " + a.Vehicle.Model,
                StaffId = a.StaffId,
                ServiceType = a.ServiceType,
                Notes = a.Notes,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<AppointmentDto>> GetAllAppointmentsAsync()
    {
        return await _db.Appointments
            .Include(a => a.Vehicle)
            .Include(a => a.Customer)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                VehicleId = a.VehicleId,
                VehiclePlate = a.Vehicle.PlateNumber,
                VehicleName = a.Vehicle.Make + " " + a.Vehicle.Model,
                StaffId = a.StaffId,
                ServiceType = a.ServiceType,
                Notes = a.Notes,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<Appointment?> GetAppointmentEntityAsync(int id)
    {
        return await _db.Appointments.FindAsync(id);
    }

    public async Task UpdateAppointmentStatusAsync(
        Appointment appointment,
        UpdateAppointmentStatusDto dto)
    {
        appointment.Status = dto.Status;

        if (dto.StaffId.HasValue)
            appointment.StaffId = dto.StaffId;

        await _db.SaveChangesAsync();
    }

    public async Task CancelAppointmentAsync(Appointment appointment)
    {
        appointment.Status = "Cancelled";

        await _db.SaveChangesAsync();
    }
}