using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Identity.Enums;
using UserDto = DrivingLicenseManagement.Application.Features.Identity.Dtos.UserResponse;

namespace DrivingLicenseManagement.Api.Contracts.Identity;

// The public shape of the identity (user account) endpoints. Controllers bind these instead of the
// MediatR commands, so the transport contract can change without dragging the application layer with it.

// *** Requests ***
public sealed record CreateUserRequest(
    Guid PersonId,
    string UserName,
    string Email,
    string Password,
    string Role = Roles.User);

public sealed record CreateUserResponse(Guid UserId);

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword);

public sealed record GetUsersRequest(
    UserFilterBy FilterBy = UserFilterBy.None,
    string? FilterValue = null,
    int PageNumber = 1,
    int PageSize = 10);

public sealed record ForgotPasswordRequest(string Email);

public sealed record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword);

// *** Responses ***

public sealed record UserResponse(
    Guid UserId,
    Guid PersonId,
    string UserName,
    string FullName,
    string Email,
    string Role,
    bool IsActive);

public sealed record GetUsersResponse(
    IReadOnlyCollection<UserResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalPages,
    int TotalCount);

// Only the paged projection is big enough to be worth a mapper; the single-value contracts are
// mapped inline at the endpoint.
public static class IdentityContractsMapper
{
    public static GetUsersResponse ToResponse(this PaginatedList<UserDto> users)
        => new(
            Items: users.Items?.Select(user => user.ToResponse()).ToList() ?? [],
            PageNumber: users.PageNumber,
            PageSize: users.PageSize,
            TotalPages: users.TotalPages,
            TotalCount: users.TotalCount);

    private static UserResponse ToResponse(this UserDto user)
        => new(
            UserId: user.UserId,
            PersonId: user.PersonId,
            UserName: user.UserName,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role,
            IsActive: user.IsActive);
}
