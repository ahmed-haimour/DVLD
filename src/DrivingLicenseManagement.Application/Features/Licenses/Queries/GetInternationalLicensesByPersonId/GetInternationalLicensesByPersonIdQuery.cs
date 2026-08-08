using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Licenses.Dtos;

namespace DrivingLicenseManagement.Application.Features.Licenses.Queries.GetInternationalLicensesByPersonId;

public sealed record GetInternationalLicensesByPersonIdQuery(Guid PersonId)
    : ICachedQuery<Result<IReadOnlyList<PersonInternationalLicenseResponse>>>
{
    public string CacheKey => $"international-licenses-by-person-{PersonId}";

    public string[] Tags => ["licenses"];
}
