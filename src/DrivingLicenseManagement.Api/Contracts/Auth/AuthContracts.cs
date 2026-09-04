using DrivingLicenseManagement.Domain.Enums;
using CurrentUserDto = DrivingLicenseManagement.Application.Features.Auth.Queries.GetCurrentUser.CurrentUserResponse;

namespace DrivingLicenseManagement.Api.Contracts.Auth;

// The public shape of the auth endpoints. Controllers bind these instead of the MediatR commands,
// so the transport contract can change without dragging the application layer with it.

public sealed record LoginRequest(string Email, string Password);

public sealed record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresOnUtc);

public sealed record LogoutRequest(string RefreshToken);

public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record RefreshTokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresOnUtc);

public sealed record GetCurrentUserResponse(
    Guid UserId,
    string UserName,
    bool IsActive,
    IList<string> Roles,
    Guid PersonId,
    string NationalNo,
    string FullName,
    DateTime DateOfBirth,
    Gender Gender,
    string Address,
    string Phone,
    string? Email,
    string Country,
    string? ImagePath);

// Only the profile projection is big enough to be worth a mapper; the token and single-field
// contracts are mapped inline at the endpoint.
public static class AuthContractsMapper
{
    public static GetCurrentUserResponse ToResponse(this CurrentUserDto user)
        => new(
            UserId: user.UserId,
            UserName: user.UserName,
            IsActive: user.IsActive,
            Roles: user.Roles,
            PersonId: user.PersonId,
            NationalNo: user.NationalNo,
            FullName: user.FullName,
            DateOfBirth: user.DateOfBirth,
            Gender: user.Gender,
            Address: user.Address,
            Phone: user.Phone,
            Email: user.Email,
            Country: user.Country,
            ImagePath: user.ImagePath);
}
