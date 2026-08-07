using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Commands.ScheduleRetakeTestAppointment;

public sealed record ScheduleRetakeTestAppointmentCommand(
    Guid LocalDrivingLicenseApplicationId,
    Guid TestTypeId,
    DateTime AppointmentDate) : IRequest<Result<Guid>>;
