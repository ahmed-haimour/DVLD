using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Licenses.Commands.IssueLocalDrivingLicense;

public sealed record IssueLocalDrivingLicenseCommand(
    Guid LocalDrivingLicenseApplicationId,
    string? Notes) : IRequest<Result<Guid>>;
