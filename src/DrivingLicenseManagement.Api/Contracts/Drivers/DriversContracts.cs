using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Drivers.Enums;
using DrivingLicenseManagement.Domain.Enums;
using DriverInternationalLicenseDto = DrivingLicenseManagement.Application.Features.Drivers.Dtos.DriverInternationalLicenseResponse;
using DriverLicenseDto = DrivingLicenseManagement.Application.Features.Drivers.Dtos.DriverLicenseResponse;
using DriverListItemDto = DrivingLicenseManagement.Application.Features.Drivers.Dtos.DriverListItemResponse;
namespace DrivingLicenseManagement.Api.Contracts.Drivers;


// Requests
public sealed record GetDriversRequest(
    DriverFilterBy FilterBy = DriverFilterBy.None,
    string? FilterValue = null,
    int PageNumber = 1,
    int PageSize = 10);


// Responses & Mappers

public sealed record DriverListItemResponse(
    Guid DriverId,
    Guid PersonId,
    string NationalNumber,
    string FullName,
    DateTime CreatedDate,
    int ActiveLicenses);


public sealed record GetDriversResponse(
    IReadOnlyCollection<DriverListItemResponse> Items,
    int PageNumber,
    int TotalPages,
    int TotalCount);


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


public sealed record DriverInternationalLicenseResponse(
    Guid InternationalLicenseId,
    Guid LicenseId,
    Guid DriverId,
    Guid ApplicationId,
    string FullName,
    string NationalNumber,
    Gender Gender,
    DateTime DateOfBirth,
    string? ImagePath,
    DateTime IssueDate,
    DateTime ExpirationDate,
    bool IsActive);


public static class DriversContractsMapper
{
    public static DriverListItemResponse ToResponse(this DriverListItemDto driver)
        => new(
            DriverId: driver.DriverId,
            PersonId: driver.PersonId,
            NationalNumber: driver.NationalNumber,
            FullName: driver.FullName,
            CreatedDate: driver.CreatedDate,
            ActiveLicenses: driver.ActiveLicenses);

    public static GetDriversResponse ToResponse(this PaginatedList<DriverListItemDto> drivers)
        => new(Items: drivers.Items!
                .Select(x => x.ToResponse())
                .ToList(),
            PageNumber: drivers.PageNumber,
            TotalPages: drivers.TotalPages,
            TotalCount: drivers.TotalCount);

    public static DriverLicenseResponse ToResponse(this DriverLicenseDto license)
        => new(
            LicenseId: license.LicenseId,
            DriverId: license.DriverId,
            LicenseClassName: license.LicenseClassName,
            FullName: license.FullName,
            NationalNumber: license.NationalNumber,
            Gender: license.Gender,
            DateOfBirth: license.DateOfBirth,
            ImagePath: license.ImagePath,
            IssueDate: license.IssueDate,
            ExpirationDate: license.ExpirationDate,
            IssueReason: license.IssueReason,
            IsActive: license.IsActive,
            IsDetained: license.IsDetained,
            Notes: license.Notes);

    public static DriverInternationalLicenseResponse ToResponse(this DriverInternationalLicenseDto license)
        => new(
            InternationalLicenseId: license.InternationalLicenseId,
            LicenseId: license.LicenseId,
            DriverId: license.DriverId,
            ApplicationId: license.ApplicationId,
            FullName: license.FullName,
            NationalNumber: license.NationalNumber,
            Gender: license.Gender,
            DateOfBirth: license.DateOfBirth,
            ImagePath: license.ImagePath,
            IssueDate: license.IssueDate,
            ExpirationDate: license.ExpirationDate,
            IsActive: license.IsActive);
}
