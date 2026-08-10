using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.DetainedLicenses.Commands.DetainLicense;

public sealed record DetainLicenseCommand(
    Guid LicenseId,
    decimal FineFees) : IRequest<Result<Guid>>;
