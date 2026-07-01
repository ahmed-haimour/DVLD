namespace DrivingLicenseManagement.Application.Features.Identity.Dtos;

public sealed record AppUserDto(
    Guid Id,
    string Email,
    string? UserName,
    Guid PersonId,
    bool IsActive,
    IReadOnlyList<string> Roles);