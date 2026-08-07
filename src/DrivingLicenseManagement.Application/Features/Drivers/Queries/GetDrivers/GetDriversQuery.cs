using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Drivers.Dtos;
using DrivingLicenseManagement.Application.Features.Drivers.Enums;

namespace DrivingLicenseManagement.Application.Features.Drivers.Queries.GetDrivers;

public sealed record GetDriversQuery(
    DriverFilterBy FilterBy = DriverFilterBy.None,
    string? FilterValue = null,
    int PageNumber = 1,
    int PageSize = 10)
    : ICachedQuery<Result<PaginatedList<DriverListItemResponse>>>
{
    private string NormalizedFilterValue => FilterBy == DriverFilterBy.None
        ? string.Empty
        : FilterValue?.Trim().ToLowerInvariant() ?? string.Empty;

    public string CacheKey => $"drivers-{FilterBy}-{NormalizedFilterValue}-{PageNumber}-{PageSize}";

    public string[] Tags => ["drivers"];
}
