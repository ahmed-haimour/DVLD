using DrivingLicenseManagement.Domain.Enums;

namespace DrivingLicenseManagement.Application.Features.Drivers.Dtos;

public sealed record DriverLicenseResponse(
    Guid LicenseId,
    Guid DriverId,
    string LicenseClassName,
    string FullName,
    string NationalNumber,
    Gender Gender,
    DateTime DateOfBirth,
    string? ImagePath,
    DateTime IssueDate,
    DateTime ExpirationDate,
    LicenseIssueReason IssueReason,
    bool IsActive,
    bool IsDetained,
    string? Notes);
