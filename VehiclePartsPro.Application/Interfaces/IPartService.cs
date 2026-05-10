using VehiclePartsPro.Application.DTOs.Part;

namespace VehiclePartsPro.Application.Interfaces;

public interface IPartService
{
    Task<List<PartDto>> GetAllPartsAsync();
    Task<PartDto?> GetPartByIdAsync(int id);
    Task<PartDto> CreatePartAsync(CreatePartDto dto);
    Task<PartDto> UpdatePartAsync(int id, UpdatePartDto dto);
    Task DeletePartAsync(int id);
    Task<List<PartDto>> GetLowStockPartsAsync();
}