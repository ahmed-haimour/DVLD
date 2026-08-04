using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.TestTypes.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.TestTypes.Queries.GetTestTypes;

public sealed class GetTestTypesQueryHandler(IAppDbContext context)
    : IRequestHandler<GetTestTypesQuery, Result<IReadOnlyList<TestTypeResponse>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<TestTypeResponse>>> Handle(
        GetTestTypesQuery request,
        CancellationToken cancellationToken)
    {
        var testTypes = await _context.TestTypes
            .AsNoTracking()
            .OrderBy(testType => testType.Id)
            .Select(testType => new TestTypeResponse(
                testType.Id,
                testType.TestTypeTitle,
                testType.TestTypeDescription,
                testType.TestTypeFees))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<TestTypeResponse>>.Success(testTypes);
    }
}
