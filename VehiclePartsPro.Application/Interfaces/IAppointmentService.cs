using VehiclePartsPro.Application.DTOs.Appointment;
using VehiclePartsPro.Domain.Entities;

namespace VehiclePartsPro.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentDto> CreateAppointmentAsync(
        string userId,
        CreateAppointmentDto dto);

    Task<List<AppointmentDto>> GetMyAppointmentsAsync(string userId);

    Task<List<AppointmentDto>> GetAllAppointmentsAsync();

    Task<Appointment?> GetAppointmentEntityAsync(int id);

    Task UpdateAppointmentStatusAsync(
        Appointment appointment,
        UpdateAppointmentStatusDto dto);

    Task CancelAppointmentAsync(Appointment appointment);
}