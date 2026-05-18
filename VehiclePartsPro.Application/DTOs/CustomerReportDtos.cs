namespace VehiclePartsPro.Application.DTOs.Customer;

public class CustomerReportDto
{
    public int CustomerId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public decimal TotalSpent { get; set; }

    public decimal CreditBalance { get; set; }

    public int TotalVehicles { get; set; }

    public int TotalOrders { get; set; }
}

public class CustomerHistoryDto
{
    public int InvoiceId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;
}