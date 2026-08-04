using DrivingLicenseManagement.Domain.Enums;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Dtos;

public sealed record LocalDrivingLicenseApplicationListItemResponse(
    Guid Id,
    string LicenseClassName,
    string NationalNumber,
    string FullName,
    DateTime ApplicationDate,
    int PassedTests,
    ApplicationStatus Status);
