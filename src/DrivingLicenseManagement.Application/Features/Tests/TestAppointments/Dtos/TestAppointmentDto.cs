namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Dtos;

public sealed record TestAppointmentDto(
    Guid Id,
    Guid TestTypeId,
    DateTime AppointmentDate,
    decimal PaidFees,
    bool IsLocked,
    bool? TestResult);
