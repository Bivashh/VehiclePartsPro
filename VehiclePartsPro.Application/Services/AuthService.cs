using VehiclePartsPro.Application.DTOs.Auth;
using VehiclePartsPro.Application.DTOs.Staff;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;

namespace VehiclePartsPro.Application.Services;

public class AuthService : IAuthService
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly ICustomerService _customerService;
    private readonly IStaffService _staffService;

    public AuthService(
        IIdentityService identityService,
        ITokenService tokenService,
        ICustomerService customerService,
        IStaffService staffService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _customerService = customerService;
        _staffService = staffService;
    }

    // =========================
    // REGISTER CUSTOMER
    // =========================
    public async Task<TokenResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto)
    {
        var user = await _identityService.CreateUserAsync(
            dto.Email,
            dto.Password,
            dto.FullName,
            "Customer");

        var customer = new Customer
        {
            UserId = user.UserId,
            Phone = dto.Phone ?? "",
            Address = dto.Address ?? ""
        };

        customer.Vehicles.Add(new Vehicle
        {
            PlateNumber = dto.PlateNumber,
            Make = dto.Make,
            Model = dto.Model,
            Year = dto.Year,
            Notes = dto.VehicleNotes
        });

        await _customerService.CreateCustomerAsync(customer);

        var role = await _identityService.GetUserRoleAsync(user.UserId);

        var token = await _tokenService.GenerateTokenAsync(
            user.UserId,
            user.FullName,
            user.Email,
            role);

        return new TokenResponseDto
        {
            Token = token,
            Role = role,
            FullName = user.FullName,
            UserId = user.UserId
        };
    }

    // =========================
    // REGISTER STAFF
    // =========================
    public async Task<TokenResponseDto> RegisterStaffAsync(RegisterStaffDto dto)
    {
        var user = await _identityService.CreateUserAsync(
            dto.Email,
            dto.Password,
            dto.FullName,
            "Staff");

        var staff = new Staff
        {
            UserId = user.UserId,
            Phone = dto.Phone,
            EmployeeCode = dto.EmployeeCode,
            HiredAt = DateTime.UtcNow
        };

        await _staffService.CreateStaffAsync(staff);

        var role = await _identityService.GetUserRoleAsync(user.UserId);

        var token = await _tokenService.GenerateTokenAsync(
            user.UserId,
            user.FullName,
            user.Email,
            role);

        return new TokenResponseDto
        {
            Token = token,
            Role = role,
            FullName = user.FullName,
            UserId = user.UserId
        };
    }

    // =========================
    // LOGIN
    // =========================
    public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
    {
            var user = await _identityService.ValidateUserAsync(
            dto.Email,
            dto.Password)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        var role = await _identityService.GetUserRoleAsync(user.UserId);

        var token = await _tokenService.GenerateTokenAsync(
            user.UserId,
            user.FullName,
            user.Email,
            role);

        return new TokenResponseDto
        {
            Token = token,
            Role = role,
            FullName = user.FullName,
            UserId = user.UserId
        };
    }
}