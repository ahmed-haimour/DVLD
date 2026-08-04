using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Commands.CreateLocalDrivingLicenseApplication;

public sealed record CreateLocalDrivingLicenseApplicationCommand(
    Guid ApplicantPersonId,
    Guid LicenseClassId) : IRequest<Result<Guid>>;
