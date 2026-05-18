using VehiclePartsPro.Application.DTOs.LowStockAlert;

namespace VehiclePartsPro.Application.Interfaces;

public interface ILowStockAlertService
{
    Task<List<LowStockAlertDto>> GetAllAlertsAsync();
    Task<List<LowStockAlertDto>> GetActiveAlertsAsync();
    Task<List<LowStockAlertDto>> GenerateLowStockAlertsAsync();
    Task<LowStockAlertDto> ResolveAlertAsync(int id);
}