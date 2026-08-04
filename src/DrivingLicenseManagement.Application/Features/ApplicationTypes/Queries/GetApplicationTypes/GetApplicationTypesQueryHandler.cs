using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.ApplicationTypes.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.ApplicationTypes.Queries.GetApplicationTypes;

public sealed class GetApplicationTypesQueryHandler(IAppDbContext context)
    : IRequestHandler<GetApplicationTypesQuery, Result<IReadOnlyList<ApplicationTypeResponse>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<ApplicationTypeResponse>>> Handle(
        GetApplicationTypesQuery request,
        CancellationToken cancellationToken)
    {
        var applicationTypes = await _context.ApplicationTypes
            .AsNoTracking()
            .OrderBy(applicationType => applicationType.Id)
            .Select(applicationType => new ApplicationTypeResponse(
                applicationType.Id,
                applicationType.ApplicationTypeTitle,
                applicationType.ApplicationTypeFees))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<ApplicationTypeResponse>>.Success(applicationTypes);
    }
}
