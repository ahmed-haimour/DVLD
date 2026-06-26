using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.People.Dtos;

namespace DrivingLicenseManagement.Application.Features.People.Queries.GetPersonByNationalNo;

public sealed record GetPersonByNationalNoQuery(string NationalNumber) : ICachedQuery<Result<PersonDto>>
{
    public string CacheKey => $"person-nationalno-{NationalNumber}";
    public string[] Tags => ["people"];
}
