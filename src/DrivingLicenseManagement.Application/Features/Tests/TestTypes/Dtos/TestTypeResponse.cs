namespace DrivingLicenseManagement.Application.Features.TestTypes.Dtos;

public sealed record TestTypeResponse(
    Guid Id,
    string TestTypeTitle,
    string TestTypeDescription,
    decimal TestTypeFees);
