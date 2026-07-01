using DrivingLicenseManagement.Domain.Enums;

namespace DrivingLicenseManagement.Application.Features.Auth.Queries.GetCurrentUser;
public sealed record CurrentUserResponse(
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
    string? ImagePath
);