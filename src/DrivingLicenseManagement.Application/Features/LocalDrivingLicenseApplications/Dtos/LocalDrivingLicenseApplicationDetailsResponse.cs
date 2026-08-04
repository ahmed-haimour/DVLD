using DrivingLicenseManagement.Domain.Enums;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Dtos;

public sealed record LocalDrivingLicenseApplicationDetailsResponse(
    Guid LocalDrivingLicenseApplicationId,
    Guid ApplicationId,
    string LicenseClassName,
    int PassedTests,
    ApplicationStatus ApplicationStatus,
    DateTime ApplicationDate,
    DateTime LastStatusDate,
    decimal PaidFees,
    string ApplicationType,
    Guid ApplicantPersonId,
    string ApplicantFullName,
    string CreatedByUserName);
