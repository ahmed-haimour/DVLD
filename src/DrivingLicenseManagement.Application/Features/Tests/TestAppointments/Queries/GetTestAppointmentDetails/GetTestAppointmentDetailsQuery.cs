using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Dtos;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Queries.GetTestAppointmentDetails;

public sealed record GetTestAppointmentDetailsQuery(
    Guid TestAppointmentId) : IRequest<Result<TestAppointmentDetailsDto>>;
