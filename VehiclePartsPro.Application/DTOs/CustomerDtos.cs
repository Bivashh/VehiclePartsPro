namespace VehiclePartsPro.Application.DTOs.Customer;

public class CustomerDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal CreditBalance { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime? CreditDueDate { get; set; }
    public List<VehicleDto> Vehicles { get; set; } = new();
}

public class VehicleDto
{
    public int Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Notes { get; set; }
}

public class UpdateCustomerProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class CustomerSearchDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? VehiclePlate { get; set; }
    public int? CustomerId { get; set; }
}