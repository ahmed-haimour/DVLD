using System.Security.Cryptography.X509Certificates;
using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Identity.Dtos;
using DrivingLicenseManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Infrastructure.Identity;

public class IdentityService(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    AppDbContext context,
    IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory) : IIdentityService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
    private readonly AppDbContext _context = context;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;

    public async Task<bool> PersonHasUserAccountAsync(Guid personId, CancellationToken ct = default)
    {
        return await _userManager.Users
            .AnyAsync(x => x.PersonId == personId, ct);
    }

    public async Task<Result<AppUserDto>> CreateUserAsync(Guid personId, string userName, string email, string password, string role, CancellationToken ct = default)
    {
        if (!await _roleManager.RoleExistsAsync(role))
            return Error.NotFound("Role_Not_Found", $"The role '{role}' does not exist.");

        var user = new AppUser
        {
            PersonId = personId,
            UserName = userName,
            Email = email,
            IsActive = true,
            EmailConfirmed = true,
        };

        // Create the user and assign the role atomically: if either step fails,
        // the transaction is disposed without committing, so nothing is persisted.
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
            return Error.Validation("User_Creation_Failed", string.Join("; ", createResult.Errors.Select(e => e.Description)));

        var roleResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
            return Error.Failure("User_Role_Assignment_Failed", string.Join("; ", roleResult.Errors.Select(e => e.Description)));

        await transaction.CommitAsync(ct);

        return new AppUserDto(user.Id, user.Email!, user.UserName, user.PersonId, user.IsActive, [role]);
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

    public async Task<IReadOnlyList<AppUserDto>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await _userManager.Users.AsNoTracking().ToListAsync(ct);

        // Bulk-load roles in a single join instead of calling GetRolesAsync per user (avoids N+1).
        var roleMap = await (
            from ur in _context.UserRoles
            join r in _context.Roles on ur.RoleId equals r.Id
            select new { ur.UserId, r.Name })
            .ToListAsync(ct);

        var rolesByUser = roleMap
            .GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.Name!).ToList());

        return users.Select(u => new AppUserDto(
            u.Id, u.Email!, u.UserName, u.PersonId, u.IsActive,
            rolesByUser.TryGetValue(u.Id, out var roles) ? roles : [])).ToList();
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
