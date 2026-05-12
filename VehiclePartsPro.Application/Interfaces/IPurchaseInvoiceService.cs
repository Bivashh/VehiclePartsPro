using VehiclePartsPro.Application.DTOs.PurchaseInvoice;

namespace VehiclePartsPro.Application.Interfaces;

public interface IPurchaseInvoiceService
{
    Task<List<PurchaseInvoiceDto>> GetAllPurchaseInvoicesAsync();
    Task<PurchaseInvoiceDto?> GetPurchaseInvoiceByIdAsync(int id);
    Task<PurchaseInvoiceDto> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceDto dto);
}
