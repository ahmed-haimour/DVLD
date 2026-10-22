using DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Dtos;
namespace DrivingLicenseManagement.Api.Contracts.TestAppointments;


// Requests
public sealed record ScheduleTestAppointmentRequest(
    Guid LocalDrivingLicenseApplicationId,
    Guid TestTypeId,
    DateTime AppointmentDate);


public sealed record GetTestAppointmentsRequest(
    Guid LocalDrivingLicenseApplicationId,
    Guid TestTypeId);


public sealed record UpdateTestAppointmentDateRequest(DateTime AppointmentDate);

public sealed record ScheduleTestAppointmentResponse(Guid TestAppointmentId);


// Responses & Mappers

public sealed record TestAppointmentResponse(
    Guid Id,
    Guid TestTypeId,
    DateTime AppointmentDate,
    decimal PaidFees,
    bool IsLocked,
    bool? TestResult);


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


public static class TestAppointmentsContractsMapper
{
    public static TestAppointmentResponse ToResponse(this TestAppointmentDto testAppointment)
        => new(
            Id: testAppointment.Id,
            TestTypeId: testAppointment.TestTypeId,
            AppointmentDate: testAppointment.AppointmentDate,
            PaidFees: testAppointment.PaidFees,
            IsLocked: testAppointment.IsLocked,
            TestResult: testAppointment.TestResult);

    public static IReadOnlyList<TestAppointmentResponse> ToResponse(this IReadOnlyList<TestAppointmentDto> testAppointments)
        => testAppointments
            .Select(x => x.ToResponse())
            .ToList();

    public static TestAppointmentDetailsResponse ToResponse(this TestAppointmentDetailsDto testAppointment)
        => new(
            Id: testAppointment.Id,
            LocalDrivingLicenseApplicationId: testAppointment.LocalDrivingLicenseApplicationId,
            TestTypeId: testAppointment.TestTypeId,
            TestTypeName: testAppointment.TestTypeName,
            AppointmentDate: testAppointment.AppointmentDate,
            TestAppointmentFees: testAppointment.TestAppointmentFees,
            RetakeApplicationFees: testAppointment.RetakeApplicationFees,
            TotalFees: testAppointment.TotalFees,
            IsLocked: testAppointment.IsLocked,
            RetakeTestApplicationId: testAppointment.RetakeTestApplicationId,
            CreatedByUserName: testAppointment.CreatedByUserName,
            TestResult: testAppointment.TestResult,
            TestNotes: testAppointment.TestNotes);
}
