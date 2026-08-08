namespace DrivingLicenseManagement.Application.Features.Licenses.Dtos;

public sealed record PersonInternationalLicenseResponse(
    Guid InternationalLicenseId,
    Guid ApplicationId,
    Guid LicenseId,
    DateTime IssueDate,
    DateTime ExpirationDate,
    bool IsActive);
