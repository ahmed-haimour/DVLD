namespace DrivingLicenseManagement.Application.Features.Drivers.Dtos;

public sealed record PersonLicenseResponse(
    Guid LicenseId,
    Guid ApplicationId,
    string ClassName,
    DateTime IssueDate,
    DateTime ExpirationDate,
    bool IsActive);
