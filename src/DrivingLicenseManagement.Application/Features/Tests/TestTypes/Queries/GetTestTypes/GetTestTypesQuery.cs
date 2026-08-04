using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.TestTypes.Dtos;

namespace DrivingLicenseManagement.Application.Features.TestTypes.Queries.GetTestTypes;

public sealed record GetTestTypesQuery
    : ICachedQuery<Result<IReadOnlyList<TestTypeResponse>>>
{
    public string CacheKey => "test-types";

    public string[] Tags => ["test-types"];
}
