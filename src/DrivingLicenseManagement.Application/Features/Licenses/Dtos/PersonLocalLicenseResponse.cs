namespace DrivingLicenseManagement.Application.Features.Licenses.Dtos;

public sealed record PersonLocalLicenseResponse(
    Guid LicenseId,
    Guid ApplicationId,
    string ClassName,
    DateTime IssueDate,
    DateTime ExpirationDate,
    bool IsActive);
