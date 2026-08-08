using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Drivers.Dtos;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Drivers.Queries.GetInternationalLicenseDriver;

public sealed record GetInternationalLicenseDriverQuery(
    Guid InternationalLicenseId) : IRequest<Result<DriverInternationalLicenseResponse>>;
