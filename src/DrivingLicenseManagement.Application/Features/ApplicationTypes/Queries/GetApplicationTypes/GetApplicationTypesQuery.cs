using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.ApplicationTypes.Dtos;

namespace DrivingLicenseManagement.Application.Features.ApplicationTypes.Queries.GetApplicationTypes;

public sealed record GetApplicationTypesQuery
    : ICachedQuery<Result<IReadOnlyList<ApplicationTypeResponse>>>
{
    public string CacheKey => "application-types";

    public string[] Tags => ["application-types"];
}
