using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.People.Dtos;
using DrivingLicenseManagement.Application.Features.People.Enums;
using DrivingLicenseManagement.Application.Features.People.Mappers;
using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.People.Queries.GetPeople;

public sealed class GetPeopleQueryHandler(IAppDbContext context)
    : IRequestHandler<GetPeopleQuery, Result<PaginatedList<PersonDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PaginatedList<PersonDto>>> Handle(GetPeopleQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Person> query = _context.People
            .AsNoTracking()
            .Include(person => person.NationalityCountry);

        var filterValue = request.FilterValue?.Trim();

        if (request.FilterBy != PersonFilterBy.None && !string.IsNullOrWhiteSpace(filterValue))
        {
            switch (request.FilterBy)
            {
                case PersonFilterBy.PersonId:
                    if (!Guid.TryParse(filterValue, out var personId))
                    {
                        return Result<PaginatedList<PersonDto>>.Failure(ApplicationErrors.InvalidPersonFilterValue);
                    }

                    query = query.Where(person => person.Id == personId);
                    break;

                case PersonFilterBy.Gender:
                    // TryParse also accepts numeric strings, so IsDefined rejects out-of-range values.
                    if (!Enum.TryParse<Gender>(filterValue, ignoreCase: true, out var gender) || !Enum.IsDefined(gender))
                    {
                        return Result<PaginatedList<PersonDto>>.Failure(ApplicationErrors.InvalidPersonFilterValue);
                    }

                    query = query.Where(person => person.Gender == gender);
                    break;

                case PersonFilterBy.NationalNo:
                    query = query.Where(person => person.NationalNumber.Contains(filterValue));
                    break;

                case PersonFilterBy.FirstName:
                    query = query.Where(person => person.FirstName.Contains(filterValue));
                    break;

                case PersonFilterBy.LastName:
                    query = query.Where(person => person.LastName.Contains(filterValue));
                    break;

                case PersonFilterBy.Nationality:
                    query = query.Where(person => person.NationalityCountry.Name.Contains(filterValue));
                    break;

                case PersonFilterBy.Phone:
                    query = query.Where(person => person.Phone.Contains(filterValue));
                    break;

                case PersonFilterBy.Email:
                    query = query.Where(person => person.Email != null && person.Email.Contains(filterValue));
                    break;
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var people = await query
            .OrderBy(person => person.LastName)
            .ThenBy(person => person.FirstName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var result = new PaginatedList<PersonDto>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            Items = people.ToDtos()
        };

        return Result<PaginatedList<PersonDto>>.Success(result);
    }
}
