using DrivingLicenseManagement.Application.Features.TestTypes.Dtos;
namespace DrivingLicenseManagement.Api.Contracts.TestTypes;


// Requests
public sealed record UpdateTestTypeRequest(
    string TestTypeTitle,
    string TestTypeDescription,
    decimal TestTypeFees);


// Responses & Mappers

public sealed record TestTypeResponse(
    Guid Id,
    string TestTypeTitle,
    string TestTypeDescription,
    decimal TestTypeFees);


public static class TestTypesContractsMapper
{
    public static TestTypeResponse ToResponse(this TestTypeDto testType)
        => new(
            Id: testType.Id,
            TestTypeTitle: testType.TestTypeTitle,
            TestTypeDescription: testType.TestTypeDescription,
            TestTypeFees: testType.TestTypeFees);

    public static IReadOnlyList<TestTypeResponse> ToResponse(this IReadOnlyList<TestTypeDto> testTypes)
        => testTypes
            .Select(x => x.ToResponse())
            .ToList();
}
