namespace DrivingLicenseManagement.Application.Features.ApplicationTypes.Dtos;

public sealed record ApplicationTypeResponse(
    Guid Id,
    string ApplicationTypeTitle,
    decimal ApplicationTypeFees);
