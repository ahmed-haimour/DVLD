namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Dtos;

public sealed record TestAppointmentDetailsResponse(
    Guid Id,
    Guid LocalDrivingLicenseApplicationId,
    Guid TestTypeId,
    string TestTypeName,
    DateTime AppointmentDate,
    decimal TestAppointmentFees,
    decimal? RetakeApplicationFees,
    decimal TotalFees,
    bool IsLocked,
    Guid? RetakeTestApplicationId,
    string CreatedByUserName,
    bool? TestResult,
    string? TestNotes);
