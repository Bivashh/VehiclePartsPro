using VehiclePartsPro.Application.DTOs.Auth;

namespace VehiclePartsPro.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto);
    Task<TokenResponseDto> RegisterStaffAsync(RegisterStaffDto dto);
    Task<TokenResponseDto> LoginAsync(LoginDto dto);
}