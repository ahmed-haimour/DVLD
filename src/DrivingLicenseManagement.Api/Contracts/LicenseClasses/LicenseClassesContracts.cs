using DrivingLicenseManagement.Application.Features.LicenseClasses.Dtos;
namespace DrivingLicenseManagement.Api.Contracts.LicenseClasses;


// Responses & Mappers

public sealed record LicenseClassResponse(
    Guid Id,
    string ClassName,
    string ClassDescription,
    byte MinimumAllowedAge,
    byte DefaultValidityLength,
    decimal ClassFees);


public static class LicenseClassesContractsMapper
{
    public static LicenseClassResponse ToResponse(this LicenseClassDto licenseClass)
        => new(
            Id: licenseClass.Id,
            ClassName: licenseClass.ClassName,
            ClassDescription: licenseClass.ClassDescription,
            MinimumAllowedAge: licenseClass.MinimumAllowedAge,
            DefaultValidityLength: licenseClass.DefaultValidityLength,
            ClassFees: licenseClass.ClassFees);

    public static IReadOnlyList<LicenseClassResponse> ToResponse(this IReadOnlyList<LicenseClassDto> licenseClasses)
        => licenseClasses
            .Select(x => x.ToResponse())
            .ToList();
}
