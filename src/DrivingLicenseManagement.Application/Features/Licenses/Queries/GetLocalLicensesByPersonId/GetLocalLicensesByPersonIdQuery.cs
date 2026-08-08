using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Licenses.Dtos;

namespace DrivingLicenseManagement.Application.Features.Licenses.Queries.GetLocalLicensesByPersonId;

public sealed record GetLocalLicensesByPersonIdQuery(Guid PersonId)
    : ICachedQuery<Result<IReadOnlyList<PersonLocalLicenseResponse>>>
{
    public string CacheKey => $"licenses-by-person-{PersonId}";

    public string[] Tags => ["licenses"];
}
