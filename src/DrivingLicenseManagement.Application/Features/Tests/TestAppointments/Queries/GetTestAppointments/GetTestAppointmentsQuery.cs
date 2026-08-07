using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Dtos;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Queries.GetTestAppointments;

public sealed record GetTestAppointmentsQuery(
    Guid LocalDrivingLicenseApplicationId,
    Guid TestTypeId) : IRequest<Result<IReadOnlyList<TestAppointmentResponse>>>;
