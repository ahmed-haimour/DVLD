using DrivingLicenseManagement.Domain.Enums;

namespace DrivingLicenseManagement.Application.Features.Drivers.Dtos;

public sealed record DriverInternationalLicenseResponse(
    Guid InternationalLicenseId,
    Guid LicenseId,
    Guid DriverId,
    Guid ApplicationId,
    string FullName,
    string NationalNumber,
    Gender Gender,
    DateTime DateOfBirth,
    string? ImagePath,
    DateTime IssueDate,
    DateTime ExpirationDate,
    bool IsActive);
