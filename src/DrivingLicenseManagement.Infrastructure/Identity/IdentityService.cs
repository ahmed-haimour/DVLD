using System.Security.Cryptography.X509Certificates;
using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Identity.Dtos;
using DrivingLicenseManagement.Application.Features.Identity.Enums;
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

    public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Failure(Error.NotFound("User_Not_Found", $"User with id '{userId}' was not found."));

        if (!user.IsActive)
            return Result.Failure(Error.Forbidden("User_Inactive", "This account has been deactivated."));

        // The new hash and the session invalidation must land together: a committed password
        // change with live refresh tokens leaves the old credential usable via refresh.
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        var changeResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!changeResult.Succeeded)
        {
            // Identity reports a wrong current password as PasswordMismatch; surface that as
            // unauthorized rather than lumping it in with password-policy failures.
            if (changeResult.Errors.Any(error => error.Code == nameof(IdentityErrorDescriber.PasswordMismatch)))
                return Result.Failure(Error.Unauthorized("Invalid_Current_Password", "The current password is incorrect."));

            return Result.Failure(Error.Validation("Password_Change_Failed", string.Join("; ", changeResult.Errors.Select(e => e.Description))));
        }

        await RevokeActiveRefreshTokensAsync(userId, ct);

        await transaction.CommitAsync(ct);

        return Result.Success();
    }

    // Shared by every credential-invalidating operation - change password today, admin reset or
    // deactivation later - so none of them can forget to kill the sessions they invalidate.
    private async Task RevokeActiveRefreshTokensAsync(Guid userId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        // IsActive is an unmapped computed property, so the revoked/expired test is spelled out
        // here to keep the filter translatable to SQL.
        var activeTokens = await _context.RefreshTokens
            .Where(token => token.UserId == userId && !token.IsRevoked && token.ExpiresAt > now)
            .ToListAsync(ct);

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = now;
        }

        if (activeTokens.Count > 0)
            await _context.SaveChangesAsync(ct);
    }

    public async Task<Result<AppUserDto>> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Error.NotFound("User_Not_Found", $"User with id '{userId}' was not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return new AppUserDto(user.Id, user.Email!, user.UserName, user.PersonId, user.IsActive, roles.ToList());
    }

    public async Task<Result<PaginatedList<UserResponse>>> GetAllUsersAsync(
        UserFilterBy filterBy, string? filterValue, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        // Explicitly typed: Where returns IQueryable<T>, which won't assign back to
        // a var-inferred query expression.
        IQueryable<AppUser> query = _userManager.Users.AsNoTracking();

        var value = filterValue?.Trim();

        if (filterBy != UserFilterBy.None && !string.IsNullOrWhiteSpace(value))
        {
            switch (filterBy)
            {
                case UserFilterBy.UserId:
                    // Search screens prefer empty results over a validation error, so an
                    // id that isn't a valid GUID simply matches nothing.
                    query = Guid.TryParse(value, out var userId)
                        ? query.Where(user => user.Id == userId)
                        : query.Where(user => false);
                    break;

                case UserFilterBy.PersonId:
                    query = Guid.TryParse(value, out var personId)
                        ? query.Where(user => user.PersonId == personId)
                        : query.Where(user => false);
                    break;

                case UserFilterBy.IsActive:
                    // A value that isn't 'true'/'false' matches nothing rather than failing validation.
                    query = bool.TryParse(value, out var isActive)
                        ? query.Where(user => user.IsActive == isActive)
                        : query.Where(user => false);
                    break;

                case UserFilterBy.Username:
                    query = query.Where(user => user.UserName != null && user.UserName.Contains(value));
                    break;

                case UserFilterBy.FullName:
                    // Every typed word must appear in some name column, so "ahmad ali" matches
                    // Ahmad Ali Hassan. Each token becomes its own ANDed Where.
                    foreach (var token in value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Where(user =>
                            user.Person.FirstName.Contains(token) ||
                            user.Person.SecondName.Contains(token) ||
                            (user.Person.ThirdName != null && user.Person.ThirdName.Contains(token)) ||
                            user.Person.LastName.Contains(token));
                    }
                    break;
            }
        }

        var totalCount = await query.CountAsync(ct);

        // Projecting the four name columns lets EF emit the join to People without
        // materializing whole Person rows.
        var users = await query
            .OrderBy(user => user.UserName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new
            {
                user.Id,
                user.PersonId,
                user.UserName,
                user.Email,
                user.IsActive,
                user.Person.FirstName,
                user.Person.SecondName,
                user.Person.ThirdName,
                user.Person.LastName,
            })
            .ToListAsync(ct);

        var userIds = users.Select(user => user.Id).ToList();

        // Bulk-load roles in a single join instead of calling GetRolesAsync per user (avoids N+1),
        // scoped to the current page rather than the whole UserRoles join.
        var roleMap = await (
            from ur in _context.UserRoles
            join r in _context.Roles on ur.RoleId equals r.Id
            where userIds.Contains(ur.UserId)
            select new { ur.UserId, r.Name })
            .ToListAsync(ct);

        var rolesByUser = roleMap
            .GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Name!).ToList());

        var items = users.Select(user => new UserResponse(
            UserId: user.Id,
            PersonId: user.PersonId,
            UserName: user.UserName ?? string.Empty,
            FullName: string.Join(" ", new[] { user.FirstName, user.SecondName, user.ThirdName, user.LastName }
                .Where(name => !string.IsNullOrWhiteSpace(name))),
            Email: user.Email ?? string.Empty,
            Role: rolesByUser.TryGetValue(user.Id, out var roles) ? roles.FirstOrDefault() ?? string.Empty : string.Empty,
            IsActive: user.IsActive)).ToList();

        return Result<PaginatedList<UserResponse>>.Success(new PaginatedList<UserResponse>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Items = items
        });
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

    public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return null;
        }

        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure(Error.Unauthorized("Invalid_Password_Reset_Request", 
                "The password reset request is invalid or has expired."));

        if (!user.IsActive)
            return Result.Failure(Error.Forbidden("User_Inactive", "This account has been deactivated."));

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!resetResult.Succeeded)
            return Result.Failure(Error.Validation("Password_Reset_Failed", string.Join("; ", resetResult.Errors.Select(e => e.Description))));

        await RevokeActiveRefreshTokensAsync(user.Id, ct);

        await transaction.CommitAsync(ct);

        return Result.Success();
    }
}
