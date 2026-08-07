using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Domain.Enums;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Licenses.Commands.ReplaceLicense;

public sealed record ReplaceLicenseCommand(
    Guid LicenseId,
    LicenseReplacementReason ReplaceReason,
    string? Notes) : IRequest<Result<Guid>>;
