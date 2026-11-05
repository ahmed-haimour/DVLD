using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.DetainedLicenses.Enums;
using DetainedLicenseListItemDto = DrivingLicenseManagement.Application.Features.DetainedLicenses.Dtos.DetainedLicenseListItemResponse;
namespace DrivingLicenseManagement.Api.Contracts.DetainedLicenses;


// Requests
public sealed record DetainLicenseRequest(
    Guid LicenseId,
    decimal FineFees);


public sealed record GetDetainedLicensesRequest(
    DetainedLicenseFilterBy FilterBy = DetainedLicenseFilterBy.None,
    string? FilterValue = null,
    int PageNumber = 1,
    int PageSize = 10);


// Responses & Mappers

public sealed record DetainLicenseResponse(Guid DetainedLicenseId);

public sealed record ReleaseDetainedLicenseResponse(Guid ReleaseApplicationId);


public sealed record DetainedLicenseListItemResponse(
    Guid DetainedLicenseId,
    Guid LicenseId,
    DateTime DetainDate,
    bool IsReleased,
    decimal FineFees,
    DateTime? ReleaseDate,
    string NationalNumber,
    string FullName,
    Guid? ReleaseApplicationId);


public sealed record GetDetainedLicensesResponse(
    IReadOnlyCollection<DetainedLicenseListItemResponse> Items,
    int PageNumber,
    int TotalPages,
    int TotalCount);


public static class DetainedLicensesContractsMapper
{
    public static DetainedLicenseListItemResponse ToResponse(this DetainedLicenseListItemDto detainedLicense)
        => new(
            DetainedLicenseId: detainedLicense.DetainedLicenseId,
            LicenseId: detainedLicense.LicenseId,
            DetainDate: detainedLicense.DetainDate,
            IsReleased: detainedLicense.IsReleased,
            FineFees: detainedLicense.FineFees,
            ReleaseDate: detainedLicense.ReleaseDate,
            NationalNumber: detainedLicense.NationalNumber,
            FullName: detainedLicense.FullName,
            ReleaseApplicationId: detainedLicense.ReleaseApplicationId);

    public static GetDetainedLicensesResponse ToResponse(this PaginatedList<DetainedLicenseListItemDto> detainedLicenses)
        => new(Items: detainedLicenses.Items!
                .Select(x => x.ToResponse())
                .ToList(),
            PageNumber: detainedLicenses.PageNumber,
            TotalPages: detainedLicenses.TotalPages,
            TotalCount: detainedLicenses.TotalCount);
}
