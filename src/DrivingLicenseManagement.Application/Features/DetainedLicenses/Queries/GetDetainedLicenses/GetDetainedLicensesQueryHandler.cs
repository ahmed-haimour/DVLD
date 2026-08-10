using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.DetainedLicenses.Dtos;
using DrivingLicenseManagement.Application.Features.DetainedLicenses.Enums;
using DrivingLicenseManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.DetainedLicenses.Queries.GetDetainedLicenses;

public sealed class GetDetainedLicensesQueryHandler(IAppDbContext context)
    : IRequestHandler<GetDetainedLicensesQuery, Result<PaginatedList<DetainedLicenseListItemResponse>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PaginatedList<DetainedLicenseListItemResponse>>> Handle(
        GetDetainedLicensesQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<DetainedLicense> query = _context.DetainedLicenses.AsNoTracking();

        var filterValue = request.FilterValue?.Trim();

        if (request.FilterBy != DetainedLicenseFilterBy.None && !string.IsNullOrWhiteSpace(filterValue))
        {
            switch (request.FilterBy)
            {
                case DetainedLicenseFilterBy.DetainId:
                    // Search screens prefer empty results over a validation error, so an
                    // id that isn't a valid GUID simply matches nothing.
                    query = Guid.TryParse(filterValue, out var detainedLicenseId)
                        ? query.Where(detainedLicense => detainedLicense.Id == detainedLicenseId)
                        : query.Where(detainedLicense => false);
                    break;

                case DetainedLicenseFilterBy.IsReleased:
                    query = bool.TryParse(filterValue, out var isReleased)
                        ? query.Where(detainedLicense => detainedLicense.IsReleased == isReleased)
                        : query.Where(detainedLicense => false);
                    break;

                case DetainedLicenseFilterBy.NationalNumber:
                    query = query.Where(detainedLicense =>
                        detainedLicense.License.Driver.Person.NationalNumber.Contains(filterValue));
                    break;

                case DetainedLicenseFilterBy.FullName:
                    // Every typed word must appear in some name column, so "ahmad ali" matches
                    // Ahmad Ali Hassan. Each token becomes its own ANDed Where.
                    foreach (var token in filterValue.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Where(detainedLicense =>
                            detainedLicense.License.Driver.Person.FirstName.Contains(token) ||
                            detainedLicense.License.Driver.Person.SecondName.Contains(token) ||
                            (detainedLicense.License.Driver.Person.ThirdName != null &&
                                detainedLicense.License.Driver.Person.ThirdName.Contains(token)) ||
                            detainedLicense.License.Driver.Person.LastName.Contains(token));
                    }

                    break;

                case DetainedLicenseFilterBy.ReleaseApplicationId:
                    query = Guid.TryParse(filterValue, out var releaseApplicationId)
                        ? query.Where(detainedLicense => detainedLicense.ReleaseApplicationId == releaseApplicationId)
                        : query.Where(detainedLicense => false);
                    break;
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var rows = await query
            .OrderByDescending(detainedLicense => detainedLicense.DetainDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(detainedLicense => new
            {
                detainedLicense.Id,
                detainedLicense.LicenseId,
                detainedLicense.DetainDate,
                detainedLicense.IsReleased,
                detainedLicense.FineFees,
                detainedLicense.ReleaseDate,
                detainedLicense.ReleaseApplicationId,
                detainedLicense.License.Driver.Person.NationalNumber,
                detainedLicense.License.Driver.Person.FirstName,
                detainedLicense.License.Driver.Person.SecondName,
                detainedLicense.License.Driver.Person.ThirdName,
                detainedLicense.License.Driver.Person.LastName,
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(row => new DetainedLicenseListItemResponse(
            row.Id,
            row.LicenseId,
            row.DetainDate,
            row.IsReleased,
            row.FineFees,
            row.ReleaseDate,
            row.NationalNumber,
            string.Join(" ", new[] { row.FirstName, row.SecondName, row.ThirdName, row.LastName }
                .Where(name => !string.IsNullOrWhiteSpace(name))),
            row.ReleaseApplicationId)).ToList();

        var result = new PaginatedList<DetainedLicenseListItemResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            Items = items,
        };

        return Result<PaginatedList<DetainedLicenseListItemResponse>>.Success(result);
    }
}
