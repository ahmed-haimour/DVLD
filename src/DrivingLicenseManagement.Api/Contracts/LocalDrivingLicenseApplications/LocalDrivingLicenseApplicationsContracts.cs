using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Enums;
using DrivingLicenseManagement.Domain.Enums;
using LocalDrivingLicenseApplicationDetailsDto = DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Dtos.LocalDrivingLicenseApplicationDetailsResponse;
using LocalDrivingLicenseApplicationListItemDto = DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Dtos.LocalDrivingLicenseApplicationListItemResponse;
namespace DrivingLicenseManagement.Api.Contracts.LocalDrivingLicenseApplications;


// Requests
public sealed record CreateLocalDrivingLicenseApplicationRequest(
    Guid ApplicantPersonId,
    Guid LicenseClassId);


public sealed record GetLocalDrivingLicenseApplicationsRequest(
    LocalDrivingLicenseApplicationFilterBy FilterBy = LocalDrivingLicenseApplicationFilterBy.None,
    string? FilterValue = null,
    int PageNumber = 1,
    int PageSize = 10);


// Responses & Mappers

public sealed record CreateLocalDrivingLicenseApplicationResponse(Guid LocalDrivingLicenseApplicationId);


public sealed record LocalDrivingLicenseApplicationListItemResponse(
    Guid Id,
    string LicenseClassName,
    string NationalNumber,
    string FullName,
    DateTime ApplicationDate,
    int PassedTests,
    ApplicationStatus Status);


public sealed record GetLocalDrivingLicenseApplicationsResponse(
    IReadOnlyCollection<LocalDrivingLicenseApplicationListItemResponse> Items,
    int PageNumber,
    int TotalPages,
    int TotalCount);


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


public static class LocalDrivingLicenseApplicationsContractsMapper
{
    public static LocalDrivingLicenseApplicationListItemResponse ToResponse(this LocalDrivingLicenseApplicationListItemDto application)
        => new(
            Id: application.Id,
            LicenseClassName: application.LicenseClassName,
            NationalNumber: application.NationalNumber,
            FullName: application.FullName,
            ApplicationDate: application.ApplicationDate,
            PassedTests: application.PassedTests,
            Status: application.Status);

    public static GetLocalDrivingLicenseApplicationsResponse ToResponse(this PaginatedList<LocalDrivingLicenseApplicationListItemDto> applications)
        => new(Items: applications.Items!
                .Select(x => x.ToResponse())
                .ToList(),
            PageNumber: applications.PageNumber,
            TotalPages: applications.TotalPages,
            TotalCount: applications.TotalCount);

    public static LocalDrivingLicenseApplicationDetailsResponse ToResponse(this LocalDrivingLicenseApplicationDetailsDto application)
        => new(
            LocalDrivingLicenseApplicationId: application.LocalDrivingLicenseApplicationId,
            ApplicationId: application.ApplicationId,
            LicenseClassName: application.LicenseClassName,
            PassedTests: application.PassedTests,
            ApplicationStatus: application.ApplicationStatus,
            ApplicationDate: application.ApplicationDate,
            LastStatusDate: application.LastStatusDate,
            PaidFees: application.PaidFees,
            ApplicationType: application.ApplicationType,
            ApplicantPersonId: application.ApplicantPersonId,
            ApplicantFullName: application.ApplicantFullName,
            CreatedByUserName: application.CreatedByUserName);
}
