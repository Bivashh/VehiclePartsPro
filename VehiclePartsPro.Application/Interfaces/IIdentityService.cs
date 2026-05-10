namespace VehiclePartsPro.Application.Interfaces;

public interface IIdentityService
{
    Task<(string UserId, string FullName, string Email)> CreateUserAsync(
        string email, string password, string fullName, string role);

    Task<(string UserId, string FullName, string Email)?> ValidateUserAsync(
        string email, string password);

    Task<string> GetUserRoleAsync(string userId);
}