using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Licenses.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.Licenses.Queries.GetInternationalLicensesByPersonId;

public sealed class GetInternationalLicensesByPersonIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetInternationalLicensesByPersonIdQuery, Result<IReadOnlyList<PersonInternationalLicenseResponse>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<PersonInternationalLicenseResponse>>> Handle(
        GetInternationalLicensesByPersonIdQuery request,
        CancellationToken cancellationToken)
    {
        var licenses = await _context.InternationalLicenses
            .AsNoTracking()
            .Where(license => license.Driver.PersonId == request.PersonId)
            .OrderByDescending(license => license.IssueDate)
            .Select(license => new PersonInternationalLicenseResponse(
                license.Id,
                license.ApplicationId,
                license.IssuedUsingLocalLicenseId,
                license.IssueDate,
                license.ExpirationDate,
                license.IsActive))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<PersonInternationalLicenseResponse>>.Success(licenses);
    }
}
