namespace VehiclePartsPro.Application.DTOs.Sales;

public class CreateSalesInvoiceDto
{
    public int CustomerId { get; set; }

    public string PaymentStatus { get; set; } = "Paid";

    public List<CreateSalesInvoiceItemDto> Items { get; set; } = new();
}

public class CreateSalesInvoiceItemDto
{
    public int PartId { get; set; }

    public int Quantity { get; set; }
}

public class SalesInvoiceResponseDto
{
    public int InvoiceId { get; set; }

    public int CustomerId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;

    public List<SalesInvoiceItemResponseDto> Items { get; set; } = new();
}

public class SalesInvoiceItemResponseDto
{
    public int PartId { get; set; }

    public string PartName { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}