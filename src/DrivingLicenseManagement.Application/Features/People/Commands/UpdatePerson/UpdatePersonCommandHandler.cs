using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.People.Commands.UpdatePerson;

public sealed class UpdatePersonCommandHandler(
    IAppDbContext context,
    HybridCache cache,
    ILogger<UpdatePersonCommandHandler> logger)
    : IRequestHandler<UpdatePersonCommand, Result>
{
    private readonly IAppDbContext _context = context;
    private readonly HybridCache _cache = cache;
    private readonly ILogger<UpdatePersonCommandHandler> _logger = logger;

    public async Task<Result> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.People
            .FirstOrDefaultAsync(person => person.Id == request.Id, cancellationToken);

        if (person is null)
        {
            _logger.LogWarning("Person with id {PersonId} was not found.", request.Id);
            return Result.Failure(ApplicationErrors.PersonNotFound);
        }

        string nationalNumber = request.NationalNumber.Trim();

        bool nationalNumberExists = await _context.People
            .AnyAsync(person => person.Id != request.Id && person.NationalNumber == nationalNumber, cancellationToken);

        if (nationalNumberExists)
        {
            _logger.LogWarning("A person with the same national number already exists.");
            return Result.Failure(ApplicationErrors.PersonNationalNumberAlreadyExists);
        }

        bool countryExists = await _context.Countries
            .AnyAsync(country => country.Id == request.NationalityCountryId, cancellationToken);

        if (!countryExists)
        {
            _logger.LogWarning("Country with id {CountryId} was not found.", request.NationalityCountryId);
            return Result.Failure(ApplicationErrors.CountryNotFound);
        }

        person.NationalNumber = nationalNumber;
        person.FirstName = request.FirstName.Trim();
        person.SecondName = request.SecondName.Trim();
        person.ThirdName = string.IsNullOrWhiteSpace(request.ThirdName) ? null : request.ThirdName.Trim();
        person.LastName = request.LastName.Trim();
        person.DateOfBirth = request.DateOfBirth;
        person.Gender = request.Gender;
        person.Address = request.Address.Trim();
        person.Phone = request.Phone.Trim();
        person.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        person.NationalityCountryId = request.NationalityCountryId;
        person.ImagePath = string.IsNullOrWhiteSpace(request.ImagePath) ? null : request.ImagePath.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Person with id {PersonId} was updated successfully.", person.Id);

        await _cache.RemoveByTagAsync("people", cancellationToken);

        return Result.Success();
    }
}
