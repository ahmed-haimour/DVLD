namespace DrivingLicenseManagement.Application.Features.Drivers.Dtos;

public sealed record DriverListItemResponse(
    Guid DriverId,
    Guid PersonId,
    string NationalNumber,
    string FullName,
    DateTime CreatedDate,
    int ActiveLicenses);
