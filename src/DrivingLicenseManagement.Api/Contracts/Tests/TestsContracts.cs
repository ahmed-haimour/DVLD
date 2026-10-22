using DrivingLicenseManagement.Application.Features.Tests.Tests.Dtos;
namespace DrivingLicenseManagement.Api.Contracts.Tests;


// Requests
public sealed record TakeTestRequest(
    Guid TestAppointmentId,
    bool TestResult,
    string? Notes);

public sealed record TakeTestResponse(Guid TestId);


// Responses & Mappers

public sealed record TestDetailsResponse(
    Guid Id,
    Guid TestAppointmentId,
    Guid TestTypeId,
    string TestTypeName,
    bool TestResult,
    string? Notes,
    DateTime AppointmentDate,
    string CreatedByUserName);


public static class TestsContractsMapper
{
    public static TestDetailsResponse ToResponse(this TestDetailsDto test)
        => new(
            Id: test.Id,
            TestAppointmentId: test.TestAppointmentId,
            TestTypeId: test.TestTypeId,
            TestTypeName: test.TestTypeName,
            TestResult: test.TestResult,
            Notes: test.Notes,
            AppointmentDate: test.AppointmentDate,
            CreatedByUserName: test.CreatedByUserName);
}
