using DrivingLicenseManagement.Domain.Enums;
using PersonInternationalLicenseDto = DrivingLicenseManagement.Application.Features.Licenses.Dtos.PersonInternationalLicenseResponse;
using PersonLocalLicenseDto = DrivingLicenseManagement.Application.Features.Licenses.Dtos.PersonLocalLicenseResponse;
namespace DrivingLicenseManagement.Api.Contracts.Licenses;


// Requests
public sealed record IssueLocalDrivingLicenseRequest(
    Guid LocalDrivingLicenseApplicationId,
    string? Notes);


public sealed record RenewLicenseRequest(
    string? Notes);


public sealed record ReplaceLicenseRequest(
    LicenseReplacementReason ReplaceReason,
    string? Notes);


// Responses & Mappers

public sealed record IssueLocalDrivingLicenseResponse(Guid LicenseId);

public sealed record RenewLicenseResponse(Guid NewLicenseId);

public sealed record ReplaceLicenseResponse(Guid NewLicenseId);

public sealed record IssueInternationalLicenseResponse(Guid InternationalLicenseId);


public sealed record PersonLocalLicenseResponse(
    Guid LicenseId,
    Guid ApplicationId,
    string ClassName,
    DateTime IssueDate,
    DateTime ExpirationDate,
    bool IsActive);


public sealed record PersonInternationalLicenseResponse(
    Guid InternationalLicenseId,
    Guid ApplicationId,
    Guid LicenseId,
    DateTime IssueDate,
    DateTime ExpirationDate,
    bool IsActive);


public static class LicensesContractsMapper
{
    public static PersonLocalLicenseResponse ToResponse(this PersonLocalLicenseDto license)
        => new(
            LicenseId: license.LicenseId,
            ApplicationId: license.ApplicationId,
            ClassName: license.ClassName,
            IssueDate: license.IssueDate,
            ExpirationDate: license.ExpirationDate,
            IsActive: license.IsActive);

    public static IReadOnlyList<PersonLocalLicenseResponse> ToResponse(this IReadOnlyList<PersonLocalLicenseDto> licenses)
        => licenses
            .Select(x => x.ToResponse()) // x represents the first license.
            .ToList();

    public static PersonInternationalLicenseResponse ToResponse(this PersonInternationalLicenseDto license)
        => new(
            InternationalLicenseId: license.InternationalLicenseId,
            ApplicationId: license.ApplicationId,
            LicenseId: license.LicenseId,
            IssueDate: license.IssueDate,
            ExpirationDate: license.ExpirationDate,
            IsActive: license.IsActive);

    public static IReadOnlyList<PersonInternationalLicenseResponse> ToResponse(this IReadOnlyList<PersonInternationalLicenseDto> licenses)
        => licenses
            .Select(x => x.ToResponse()) // x represents the first license.
            .ToList();
}
