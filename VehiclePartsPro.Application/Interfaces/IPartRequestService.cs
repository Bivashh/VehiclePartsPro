using VehiclePartsPro.Application.DTOs.PartRequest;

namespace VehiclePartsPro.Application.Interfaces;

public interface IPartRequestService
{
    Task<PartRequestDto> CreatePartRequestAsync(string userId, CreatePartRequestDto dto);
    Task<List<PartRequestDto>> GetMyPartRequestsAsync(string userId);
    Task<List<PartRequestDto>> GetAllPartRequestsAsync();
    Task<PartRequestDto?> UpdateStatusAsync(int id, UpdatePartRequestStatusDto dto);
}