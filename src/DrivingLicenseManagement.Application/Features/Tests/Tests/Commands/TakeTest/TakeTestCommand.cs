using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Tests.Tests.Commands.TakeTest;

public sealed record TakeTestCommand(
    Guid TestAppointmentId,
    bool TestResult,
    string? Notes) : IRequest<Result<Guid>>;
