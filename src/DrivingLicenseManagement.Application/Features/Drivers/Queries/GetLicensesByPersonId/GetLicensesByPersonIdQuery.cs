using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Drivers.Dtos;

namespace DrivingLicenseManagement.Application.Features.Drivers.Queries.GetLicensesByPersonId;

public sealed record GetLicensesByPersonIdQuery(Guid PersonId)
    : ICachedQuery<Result<IReadOnlyList<PersonLicenseResponse>>>
{
    public string CacheKey => $"licenses-by-person-{PersonId}";

    public string[] Tags => ["licenses"];
}
