namespace VehiclePartsPro.Application.DTOs.Staff;

public class StaffDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public DateTime HiredAt { get; set; }
}

public class UpdateStaffDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}