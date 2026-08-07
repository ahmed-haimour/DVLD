using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Licenses.Commands.RenewLicense;

public sealed record RenewLicenseCommand(
    Guid LicenseId,
    string? Notes) : IRequest<Result<Guid>>;
