namespace DrivingLicenseManagement.Application.Features.Tests.Tests.Dtos;

public sealed record TestDetailsResponse(
    Guid Id,
    Guid TestAppointmentId,
    Guid TestTypeId,
    string TestTypeName,
    bool TestResult,
    string? Notes,
    DateTime AppointmentDate,
    string CreatedByUserName);
