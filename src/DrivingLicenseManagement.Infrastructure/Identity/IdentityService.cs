using System.Security.Cryptography.X509Certificates;
using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Identity.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Infrastructure.Identity;

public class IdentityService(
    UserManager<AppUser> userManager,
    IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory) : IIdentityService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;

    public async Task<bool> PersonHasUserAccountAsync(Guid personId, CancellationToken ct = default)
    {
        return await _userManager.Users
            .AnyAsync(x => x.PersonId == personId, ct);
    }

    public async Task<Result<AppUserDto>> AuthenticateAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Error.NotFound("User_Not_Found", $"User with email {UtilityService.MaskEmail(email)} not found");

        if (!user.IsActive)
            return Error.Forbidden("User_Inactive", "This account has been deactivated.");

        if (!user.EmailConfirmed)
            return Error.Conflict("Email_Not_Confirmed", $"email '{UtilityService.MaskEmail(email)}' not confirmed");

        if (!await _userManager.CheckPasswordAsync(user, password))
            return Error.Unauthorized("Invalid_Login_Attempt", "Email / Password are incorrect");

        return new AppUserDto(user.Id, user.Email!, user.UserName, user.PersonId, user.IsActive, (await _userManager.GetRolesAsync(user)).ToList()!);
    }

    public async Task<Result<AppUserDto>> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Error.NotFound("User_Not_Found", $"User with id '{userId}' was not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return new AppUserDto(user.Id, user.Email!, user.UserName, user.PersonId, user.IsActive, roles.ToList());
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user is not null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(Guid userId, string policyName, CancellationToken ct = default)
    {
        throw new Exception();
    }

}
