using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.People.Dtos;

namespace DrivingLicenseManagement.Application.Features.People.Queries.GetPeople;

public sealed record GetPeopleQuery(int PageNumber = 1, int PageSize = 10)
    : ICachedQuery<Result<PaginatedList<PersonDto>>>
{
    public string CacheKey => $"people-{PageNumber}-{PageSize}";
    public string[] Tags => ["people"];
}
