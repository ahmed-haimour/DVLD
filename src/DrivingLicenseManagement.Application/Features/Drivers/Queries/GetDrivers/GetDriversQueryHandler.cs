using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Drivers.Dtos;
using DrivingLicenseManagement.Application.Features.Drivers.Enums;
using DrivingLicenseManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.Drivers.Queries.GetDrivers;

public sealed class GetDriversQueryHandler(IAppDbContext context)
    : IRequestHandler<GetDriversQuery, Result<PaginatedList<DriverListItemResponse>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PaginatedList<DriverListItemResponse>>> Handle(
        GetDriversQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Driver> query = _context.Drivers.AsNoTracking();

        var filterValue = request.FilterValue?.Trim();

        if (request.FilterBy != DriverFilterBy.None && !string.IsNullOrWhiteSpace(filterValue))
        {
            switch (request.FilterBy)
            {
                case DriverFilterBy.DriverId:
                    // Search screens prefer empty results over a validation error, so an
                    // id that isn't a valid GUID simply matches nothing.
                    query = Guid.TryParse(filterValue, out var driverId)
                        ? query.Where(driver => driver.Id == driverId)
                        : query.Where(driver => false);
                    break;

                case DriverFilterBy.PersonId:
                    query = Guid.TryParse(filterValue, out var personId)
                        ? query.Where(driver => driver.PersonId == personId)
                        : query.Where(driver => false);
                    break;

                case DriverFilterBy.NationalNo:
                    query = query.Where(driver => driver.Person.NationalNumber.Contains(filterValue));
                    break;

                case DriverFilterBy.FullName:
                    // Every typed word must appear in some name column, so "ahmad ali" matches
                    // Ahmad Ali Hassan. Each token becomes its own ANDed Where.
                    foreach (var token in filterValue.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Where(driver =>
                            driver.Person.FirstName.Contains(token) ||
                            driver.Person.SecondName.Contains(token) ||
                            (driver.Person.ThirdName != null && driver.Person.ThirdName.Contains(token)) ||
                            driver.Person.LastName.Contains(token));
                    }

                    break;
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Projecting the four name columns lets EF emit the join without materializing whole rows.
        var rows = await query
            .OrderBy(driver => driver.Person.LastName)
            .ThenBy(driver => driver.Person.FirstName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(driver => new
            {
                driver.Id,
                driver.PersonId,
                driver.Person.NationalNumber,
                driver.Person.FirstName,
                driver.Person.SecondName,
                driver.Person.ThirdName,
                driver.Person.LastName,
                driver.CreatedDate,
                ActiveLicenses = driver.Licenses.Count(license => license.IsActive),
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(row => new DriverListItemResponse(
            row.Id,
            row.PersonId,
            row.NationalNumber,
            string.Join(" ", new[] { row.FirstName, row.SecondName, row.ThirdName, row.LastName }
                .Where(name => !string.IsNullOrWhiteSpace(name))),
            row.CreatedDate,
            row.ActiveLicenses)).ToList();

        var result = new PaginatedList<DriverListItemResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            Items = items
        };

        return Result<PaginatedList<DriverListItemResponse>>.Success(result);
    }
}
