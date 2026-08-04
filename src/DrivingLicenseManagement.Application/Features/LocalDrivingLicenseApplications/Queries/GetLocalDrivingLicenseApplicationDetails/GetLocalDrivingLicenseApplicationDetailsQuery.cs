using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Dtos;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Queries.GetLocalDrivingLicenseApplicationDetails;

public sealed record GetLocalDrivingLicenseApplicationDetailsQuery(
    Guid LocalDrivingLicenseApplicationId)
    : IRequest<Result<LocalDrivingLicenseApplicationDetailsResponse>>;
