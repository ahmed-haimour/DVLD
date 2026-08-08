using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Licenses.Commands.IssueInternationalLicense;

public sealed record IssueInternationalLicenseCommand(
    Guid LicenseId) : IRequest<Result<Guid>>;
