using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.DetainedLicenses.Commands.ReleaseDetainedLicense;

public sealed record ReleaseDetainedLicenseCommand(
    Guid DetainedLicenseId) : IRequest<Result<Guid>>;
