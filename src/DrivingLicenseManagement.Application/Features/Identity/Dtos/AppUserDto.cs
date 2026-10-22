namespace DrivingLicenseManagement.Application.Features.Identity.Dtos;

public sealed record AppUserDto(
    Guid Id,
    string Email,
    string? UserName,
    Guid PersonId,
    bool IsActive,
    bool EmailConfirmed,
    IReadOnlyList<string> Roles);