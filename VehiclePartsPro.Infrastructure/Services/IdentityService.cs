using Microsoft.AspNetCore.Identity;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Infrastructure.Identity;

namespace VehiclePartsPro.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;

    public IdentityService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    //  CREATE USER
    public async Task<(string UserId, string FullName, string Email)> CreateUserAsync(
        string email, string password, string fullName, string role)
    {
        var user = new User
        {
            Email = email,
            UserName = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, role);

        return (user.Id, user.FullName, user.Email!);
    }

    //  LOGIN VALIDATION
    public async Task<(string UserId, string FullName, string Email)?> ValidateUserAsync(
        string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return null;

        var valid = await _userManager.CheckPasswordAsync(user, password);

        if (!valid)
            return null;

        return (user.Id, user.FullName, user.Email!);
    }

    //  GET ROLE
    public async Task<string> GetUserRoleAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new Exception("User not found");

        var roles = await _userManager.GetRolesAsync(user);

        return roles.FirstOrDefault() ?? "Customer";
    }
}