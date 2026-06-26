using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.People.Dtos;
using DrivingLicenseManagement.Application.Features.People.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.People.Queries.GetPersonByNationalNo;

public sealed class GetPersonByNationalNoQueryHandler(IAppDbContext context, ILogger<GetPersonByNationalNoQueryHandler> logger)
    : IRequestHandler<GetPersonByNationalNoQuery, Result<PersonDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetPersonByNationalNoQueryHandler> _logger = logger;

    public async Task<Result<PersonDto>> Handle(GetPersonByNationalNoQuery request, CancellationToken cancellationToken)
    {
        var nationalNumber = request.NationalNumber.Trim();

        var person = await _context.People
            .AsNoTracking()
            .FirstOrDefaultAsync(person => person.NationalNumber == nationalNumber, cancellationToken);

        if (person is null)
        {
            _logger.LogWarning("Person with national number {NationalNumber} was not found.", nationalNumber);

            return Result<PersonDto>.Failure(ApplicationErrors.PersonNotFound);
        }

        return Result<PersonDto>.Success(person.ToDto());
    }
}
