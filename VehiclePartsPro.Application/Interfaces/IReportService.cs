using VehiclePartsPro.Application.DTOs;

namespace VehiclePartsPro.Application.Interfaces;

public interface IReportService
{
    Task<FinancialSummaryDto> GetFinancialSummaryAsync();

    Task<List<MonthlySalesDto>> GetMonthlySalesReportAsync();

    Task<List<TopSellingPartDto>> GetTopSellingPartsAsync();
}