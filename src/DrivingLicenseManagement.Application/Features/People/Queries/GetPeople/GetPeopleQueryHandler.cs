using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.People.Dtos;
using DrivingLicenseManagement.Application.Features.People.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.People.Queries.GetPeople;

public sealed class GetPeopleQueryHandler(IAppDbContext context)
    : IRequestHandler<GetPeopleQuery, Result<PaginatedList<PersonDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PaginatedList<PersonDto>>> Handle(GetPeopleQuery request, CancellationToken cancellationToken)
    {
        var query = _context.People
            .AsNoTracking()
            .OrderBy(person => person.LastName)
            .ThenBy(person => person.FirstName);

        var totalCount = await query.CountAsync(cancellationToken);

        var people = await query
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
