namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Dtos;

public sealed record TestAppointmentResponse(
    Guid Id,
    Guid TestTypeId,
    DateTime AppointmentDate,
    decimal PaidFees,
    bool IsLocked,
    bool? TestResult);
