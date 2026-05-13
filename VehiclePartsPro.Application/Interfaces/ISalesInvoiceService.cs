using VehiclePartsPro.Application.DTOs.Sales;

namespace VehiclePartsPro.Application.Interfaces;

public interface ISalesInvoiceService
{
    Task<SalesInvoiceResponseDto> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto);

    Task<List<SalesInvoiceResponseDto>> GetAllSalesInvoicesAsync();

    Task<SalesInvoiceResponseDto?> GetSalesInvoiceByIdAsync(int id);
}