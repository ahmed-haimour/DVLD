using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Licenses.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.Licenses.Queries.GetLocalLicensesByPersonId;

public sealed class GetLocalLicensesByPersonIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetLocalLicensesByPersonIdQuery, Result<IReadOnlyList<PersonLocalLicenseResponse>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<PersonLocalLicenseResponse>>> Handle(
        GetLocalLicensesByPersonIdQuery request,
        CancellationToken cancellationToken)
    {
        var licenses = await _context.Licenses
            .AsNoTracking()
            .Where(license => license.Driver.PersonId == request.PersonId)
            .OrderByDescending(license => license.IssueDate)
            .Select(license => new PersonLocalLicenseResponse(
                license.Id,
                license.ApplicationId,
                license.LicenseClass.ClassName,
                license.IssueDate,
                license.ExpirationDate,
                license.IsActive))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<PersonLocalLicenseResponse>>.Success(licenses);
    }
}
