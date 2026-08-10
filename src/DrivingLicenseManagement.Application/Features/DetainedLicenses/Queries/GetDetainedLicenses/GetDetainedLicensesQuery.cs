using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.DetainedLicenses.Dtos;
using DrivingLicenseManagement.Application.Features.DetainedLicenses.Enums;

namespace DrivingLicenseManagement.Application.Features.DetainedLicenses.Queries.GetDetainedLicenses;

public sealed record GetDetainedLicensesQuery(
    DetainedLicenseFilterBy FilterBy = DetainedLicenseFilterBy.None,
    string? FilterValue = null,
    int PageNumber = 1,
    int PageSize = 10)
    : ICachedQuery<Result<PaginatedList<DetainedLicenseListItemResponse>>>
{
    // Normalized so a leftover FilterValue can't fork the cache when no filter is selected.
    private string NormalizedFilterValue => FilterBy == DetainedLicenseFilterBy.None
        ? string.Empty
        : FilterValue?.Trim().ToLowerInvariant() ?? string.Empty;

    public string CacheKey => $"detained-licenses-{FilterBy}-{NormalizedFilterValue}-{PageNumber}-{PageSize}";

    public string[] Tags => ["detained-licenses"];
}
