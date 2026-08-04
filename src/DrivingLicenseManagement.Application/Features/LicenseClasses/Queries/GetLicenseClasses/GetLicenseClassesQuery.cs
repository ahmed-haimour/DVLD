using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.LicenseClasses.Dtos;

namespace DrivingLicenseManagement.Application.Features.LicenseClasses.Queries.GetLicenseClasses;

public sealed record GetLicenseClassesQuery
    : ICachedQuery<Result<IReadOnlyList<LicenseClassDto>>>
{
    public string CacheKey => "license-classes";

    public string[] Tags => ["license-classes"];
}