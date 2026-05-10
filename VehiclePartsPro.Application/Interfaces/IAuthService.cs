using VehiclePartsPro.Application.DTOs.Auth;
using VehiclePartsPro.Application.DTOs.Staff;

namespace VehiclePartsPro.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto);

    Task<TokenResponseDto> RegisterStaffAsync(RegisterStaffDto dto);

    Task<TokenResponseDto> LoginAsync(LoginDto dto);
}