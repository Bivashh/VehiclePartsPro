using VehiclePartsPro.Application.DTOs.Vendor;

namespace VehiclePartsPro.Application.Interfaces;

public interface IVendorService
{
    Task<List<VendorDto>> GetAllVendorsAsync();
    Task<VendorDto?> GetVendorByIdAsync(int id);
    Task<VendorDto> CreateVendorAsync(CreateVendorDto dto);
    Task<VendorDto> UpdateVendorAsync(int id, UpdateVendorDto dto);
    Task DeleteVendorAsync(int id);
}