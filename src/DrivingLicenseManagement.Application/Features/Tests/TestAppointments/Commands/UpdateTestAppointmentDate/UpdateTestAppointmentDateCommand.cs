using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Commands.UpdateTestAppointmentDate;

public sealed record UpdateTestAppointmentDateCommand(
    Guid TestAppointmentId,
    DateTime AppointmentDate) : IRequest<Result>;
