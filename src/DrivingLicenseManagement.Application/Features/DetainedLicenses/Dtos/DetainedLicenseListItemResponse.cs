namespace DrivingLicenseManagement.Application.Features.DetainedLicenses.Dtos;

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
