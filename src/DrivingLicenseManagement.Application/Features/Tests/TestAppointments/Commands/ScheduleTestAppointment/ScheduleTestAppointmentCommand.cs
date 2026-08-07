using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Commands.ScheduleTestAppointment;

public sealed record ScheduleTestAppointmentCommand(
    Guid LocalDrivingLicenseApplicationId,
    Guid TestTypeId,
    DateTime AppointmentDate) : IRequest<Result<Guid>>;
