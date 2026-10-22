using DrivingLicenseManagement.Application.Features.ApplicationTypes.Dtos;
namespace DrivingLicenseManagement.Api.Contracts.ApplicationTypes;


// Requests
public sealed record UpdateApplicationTypeRequest(
    string ApplicationTypeTitle,
    decimal ApplicationTypeFees);


// Responses & Mappers

public sealed record ApplicationTypeResponse(
    Guid Id,
    string ApplicationTypeTitle,
    decimal ApplicationTypeFees);


public static class ApplicationTypesContractsMapper
{
    public static ApplicationTypeResponse ToResponse(this ApplicationTypeDto applicationType)
        => new(
            Id: applicationType.Id,
            ApplicationTypeTitle: applicationType.ApplicationTypeTitle,
            ApplicationTypeFees: applicationType.ApplicationTypeFees);

    public static IReadOnlyList<ApplicationTypeResponse> ToResponse(this IReadOnlyList<ApplicationTypeDto> applicationTypes)
        => applicationTypes
            .Select(x => x.ToResponse())
            .ToList();
}
