using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Drivers.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.Drivers.Queries.GetDriverLicense;

public sealed class GetDriverLicenseQueryHandler(IAppDbContext context)
    : IRequestHandler<GetDriverLicenseQuery, Result<DriverLicenseResponse>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<DriverLicenseResponse>> Handle(
        GetDriverLicenseQuery request,
        CancellationToken cancellationToken)
    {
        var row = await _context.Licenses
            .AsNoTracking()
            .Where(license => license.Id == request.LicenseId)
            .Select(license => new
            {
                license.Id,
                license.DriverId,
                LicenseClassName = license.LicenseClass.ClassName,
                license.Driver.Person.FirstName,
                license.Driver.Person.SecondName,
                license.Driver.Person.ThirdName,
                license.Driver.Person.LastName,
                license.Driver.Person.NationalNumber,
                license.Driver.Person.Gender,
                license.Driver.Person.DateOfBirth,
                license.Driver.Person.ImagePath,
                license.IssueDate,
                license.ExpirationDate,
                license.IssueReason,
                license.IsActive,
                IsDetained = license.Detentions.Any(detention => !detention.IsReleased),
                license.Notes,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return Result<DriverLicenseResponse>.Failure(ApplicationErrors.LicenseNotFound);
        }

        var fullName = string.Join(" ", new[] { row.FirstName, row.SecondName, row.ThirdName, row.LastName }
            .Where(name => !string.IsNullOrWhiteSpace(name)));

        var response = new DriverLicenseResponse(
            row.Id,
            row.DriverId,
            row.LicenseClassName,
            fullName,
            row.NationalNumber,
            row.Gender,
            row.DateOfBirth,
            row.ImagePath,
            row.IssueDate,
            row.ExpirationDate,
            row.IssueReason,
            row.IsActive,
            row.IsDetained,
            row.Notes);

        return Result<DriverLicenseResponse>.Success(response);
    }
}
