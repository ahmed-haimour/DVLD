using DrivingLicenseManagement.Application.Features.LicenseClasses.Dtos;
using DrivingLicenseManagement.Domain.Entities;

namespace DrivingLicenseManagement.Application.Features.LicenseClasses.Mappers;

public static class LicenseClassMapper
{
    public static LicenseClassDto ToDto(this LicenseClass licenseClass)
    {
        return new LicenseClassDto(
            licenseClass.Id,
            licenseClass.ClassName,
            licenseClass.ClassDescription,
            licenseClass.MinimumAllowedAge,
            licenseClass.DefaultValidityLength,
            licenseClass.ClassFees);
    }

    public static List<LicenseClassDto> ToDtos(this IEnumerable<LicenseClass> licenseClasses)
    {
        return [.. licenseClasses.Select(licenseClass => licenseClass.ToDto())];
    }
}