using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Drivers.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.Drivers.Queries.GetInternationalLicenseDriver;

public sealed class GetInternationalLicenseDriverQueryHandler(IAppDbContext context)
    : IRequestHandler<GetInternationalLicenseDriverQuery, Result<DriverInternationalLicenseResponse>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<DriverInternationalLicenseResponse>> Handle(
        GetInternationalLicenseDriverQuery request,
        CancellationToken cancellationToken)
    {
        var row = await _context.InternationalLicenses
            .AsNoTracking()
            .Where(license => license.Id == request.InternationalLicenseId)
            .Select(license => new
            {
                license.Id,
                license.IssuedUsingLocalLicenseId,
                license.DriverId,
                license.ApplicationId,
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
                license.IsActive,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return Result<DriverInternationalLicenseResponse>.Failure(ApplicationErrors.InternationalLicenseNotFound);
        }

        var fullName = string.Join(" ", new[] { row.FirstName, row.SecondName, row.ThirdName, row.LastName }
            .Where(name => !string.IsNullOrWhiteSpace(name)));

        var response = new DriverInternationalLicenseResponse(
            row.Id,
            row.IssuedUsingLocalLicenseId,
            row.DriverId,
            row.ApplicationId,
            fullName,
            row.NationalNumber,
            row.Gender,
            row.DateOfBirth,
            row.ImagePath,
            row.IssueDate,
            row.ExpirationDate,
            row.IsActive);

        return Result<DriverInternationalLicenseResponse>.Success(response);
    }
}
