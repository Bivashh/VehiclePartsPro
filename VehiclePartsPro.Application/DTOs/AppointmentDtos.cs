namespace VehiclePartsPro.Application.DTOs.Appointment;

public class CreateAppointmentDto
{
    public int VehicleId { get; set; }

    public string ServiceType { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime AppointmentDate { get; set; }
}

public class UpdateAppointmentStatusDto
{
    public string Status { get; set; } = string.Empty;

    public int? StaffId { get; set; }
}

public class AppointmentDto
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public int VehicleId { get; set; }

    public string VehiclePlate { get; set; } = string.Empty;

    public string VehicleName { get; set; } = string.Empty;

    public int? StaffId { get; set; }

    public string ServiceType { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}