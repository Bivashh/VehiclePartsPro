namespace VehiclePartsPro.Application.DTOs;

public class FinancialSummaryDto
{
    public decimal TotalSalesRevenue { get; set; }

    public decimal TotalPurchaseCost { get; set; }

    public decimal EstimatedProfit { get; set; }

    public int TotalSalesInvoices { get; set; }

    public int TotalPurchaseInvoices { get; set; }

    public int TotalPartsSold { get; set; }

    public int TotalPartsPurchased { get; set; }
}

public class MonthlySalesDto
{
    public string Month { get; set; } = string.Empty;

    public decimal SalesRevenue { get; set; }
}

public class TopSellingPartDto
{
    public int PartId { get; set; }

    public string PartName { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;

    public int QuantitySold { get; set; }
}