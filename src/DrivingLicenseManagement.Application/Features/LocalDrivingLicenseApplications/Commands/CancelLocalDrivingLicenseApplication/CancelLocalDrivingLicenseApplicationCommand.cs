using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Commands.CancelLocalDrivingLicenseApplication;

public sealed record CancelLocalDrivingLicenseApplicationCommand(
    Guid LocalDrivingLicenseApplicationId) : IRequest<Result>;
