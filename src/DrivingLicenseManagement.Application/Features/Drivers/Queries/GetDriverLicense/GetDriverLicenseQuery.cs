using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Drivers.Dtos;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Drivers.Queries.GetDriverLicense;

public sealed record GetDriverLicenseQuery(
    Guid LicenseId) : IRequest<Result<DriverLicenseResponse>>;
