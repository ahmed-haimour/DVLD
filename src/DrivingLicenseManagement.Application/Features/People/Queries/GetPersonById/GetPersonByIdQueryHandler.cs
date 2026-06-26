using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.People.Dtos;
using DrivingLicenseManagement.Application.Features.People.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.People.Queries.GetPersonById;

public sealed class GetPersonByIdQueryHandler(IAppDbContext context, ILogger<GetPersonByIdQueryHandler> logger)
    : IRequestHandler<GetPersonByIdQuery, Result<PersonDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetPersonByIdQueryHandler> _logger = logger;

    public async Task<Result<PersonDto>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person  = await _context.People.AsNoTracking()
        .FirstOrDefaultAsync(p=>p.Id == request.Id, cancellationToken);

        if(person is null)
        {
            _logger.LogWarning("Person with id {PersonId} was not found.", request.Id);
            
            return Result<PersonDto>.Failure(ApplicationErrors.PersonNotFound);
        }

        return Result<PersonDto>.Success(person.ToDto());
    }
}
