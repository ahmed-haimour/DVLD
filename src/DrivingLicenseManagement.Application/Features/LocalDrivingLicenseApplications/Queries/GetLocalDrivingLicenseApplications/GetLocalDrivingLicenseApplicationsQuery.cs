using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Dtos;
using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Enums;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Queries.GetLocalDrivingLicenseApplications;

public sealed record GetLocalDrivingLicenseApplicationsQuery(
    LocalDrivingLicenseApplicationFilterBy FilterBy = LocalDrivingLicenseApplicationFilterBy.None,
    string? FilterValue = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<LocalDrivingLicenseApplicationListItemResponse>>>;
