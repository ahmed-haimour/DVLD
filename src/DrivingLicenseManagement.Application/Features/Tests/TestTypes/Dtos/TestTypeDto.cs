namespace DrivingLicenseManagement.Application.Features.TestTypes.Dtos;

public sealed record TestTypeDto(
    Guid Id,
    string TestTypeTitle,
    string TestTypeDescription,
    decimal TestTypeFees);
