using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Drivers.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.Drivers.Queries.GetLicensesByPersonId;

public sealed class GetLicensesByPersonIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetLicensesByPersonIdQuery, Result<IReadOnlyList<PersonLicenseResponse>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<PersonLicenseResponse>>> Handle(
        GetLicensesByPersonIdQuery request,
        CancellationToken cancellationToken)
    {
        var licenses = await _context.Licenses
            .AsNoTracking()
            .Where(license => license.Driver.PersonId == request.PersonId)
            .OrderByDescending(license => license.IssueDate)
            .Select(license => new PersonLicenseResponse(
                license.Id,
                license.ApplicationId,
                license.LicenseClass.ClassName,
                license.IssueDate,
                license.ExpirationDate,
                license.IsActive))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<PersonLicenseResponse>>.Success(licenses);
    }
}
