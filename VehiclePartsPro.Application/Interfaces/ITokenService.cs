namespace VehiclePartsPro.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(string userId, string fullName, string email, string role);
}