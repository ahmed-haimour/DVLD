namespace DrivingLicenseManagement.Application.Features.Identity.Dtos;

public sealed record UserResponse(
    Guid UserId,
    Guid PersonId,
    string UserName,
    string FullName,
    string Email,
    string Role,
    bool IsActive);