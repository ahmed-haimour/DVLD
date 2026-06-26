using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.People.Dtos;

namespace DrivingLicenseManagement.Application.Features.People.Queries.GetPersonById;

public sealed record GetPersonByIdQuery(Guid Id) : ICachedQuery<Result<PersonDto>>
{
    public string CacheKey => $"person-{Id}";
    public string[] Tags => ["people"];
}
