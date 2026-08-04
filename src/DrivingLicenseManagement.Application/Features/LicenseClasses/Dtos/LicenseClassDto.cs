namespace DrivingLicenseManagement.Application.Features.LicenseClasses.Dtos;

public sealed record LicenseClassDto(
    Guid Id,
    string ClassName,
    string ClassDescription,
    byte MinimumAllowedAge,
    byte DefaultValidityLength,
    decimal ClassFees);