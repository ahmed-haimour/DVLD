using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.ApplicationTypes.Commands.UpdateApplicationType;

public sealed record UpdateApplicationTypeCommand(
    Guid Id,
    string ApplicationTypeTitle,
    decimal ApplicationTypeFees) : IRequest<Result>;
