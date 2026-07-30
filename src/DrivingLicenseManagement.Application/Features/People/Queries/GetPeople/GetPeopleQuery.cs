using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.People.Dtos;
using DrivingLicenseManagement.Application.Features.People.Enums;

namespace DrivingLicenseManagement.Application.Features.People.Queries.GetPeople;

public sealed record GetPeopleQuery(
    PersonFilterBy FilterBy = PersonFilterBy.None,
    string? FilterValue = null,
    int PageNumber = 1,
    int PageSize = 10)
    : ICachedQuery<Result<PaginatedList<PersonDto>>>
{
    // Normalized so "Ahmad" and " ahmad " share one entry, and so a leftover FilterValue
    // can't fork the cache when no filter is selected.
    private string NormalizedFilterValue => FilterBy == PersonFilterBy.None
        ? string.Empty
        : FilterValue?.Trim().ToLowerInvariant() ?? string.Empty;

    public string CacheKey => $"people-{FilterBy}-{NormalizedFilterValue}-{PageNumber}-{PageSize}";
    public string[] Tags => ["people"];
}
