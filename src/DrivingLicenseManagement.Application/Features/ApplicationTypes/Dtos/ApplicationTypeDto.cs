namespace DrivingLicenseManagement.Application.Features.ApplicationTypes.Dtos;

public sealed record ApplicationTypeDto(
    Guid Id,
    string ApplicationTypeTitle,
    decimal ApplicationTypeFees);
