namespace VehiclePartsPro.Application.DTOs;

public class OverduePaymentReminderDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal CreditBalance { get; set; }
    public DateTime? CreditDueDate { get; set; }
    public string Status { get; set; } = string.Empty;
}