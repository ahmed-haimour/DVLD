using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.People.Commands.CreatePerson;

public sealed class CreatePersonCommandHandler(
    IAppDbContext context,
    ILogger<CreatePersonCommandHandler> logger,
    HybridCache cache)
    : IRequestHandler<CreatePersonCommand, Result<Guid>>
{

    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreatePersonCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Guid>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        string nationalNumber = request.NationalNumber.Trim();

        bool nationalNumberExists = await _context.People
            .AnyAsync(person => person.NationalNumber == nationalNumber, cancellationToken);

        if (nationalNumberExists)
        {
            _logger.LogWarning("A person with the same national number already exists.");
            
            return Result<Guid>.Failure(ApplicationErrors.PersonNationalNumberAlreadyExists);
        }

        bool countryExists = await _context.Countries
            .AnyAsync(country => country.Id == request.NationalityCountryId, cancellationToken);

        if (!countryExists)
        {
            _logger.LogWarning("Country with id {CountryId} was not found.", request.NationalityCountryId);

            return Result<Guid>.Failure(ApplicationErrors.CountryNotFound);
        }

        var person = new Person
        {
            NationalNumber = nationalNumber,
            FirstName = request.FirstName.Trim(),
            SecondName = request.SecondName.Trim(),
            ThirdName = string.IsNullOrWhiteSpace(request.ThirdName) ? null : request.ThirdName.Trim(),
            LastName = request.LastName.Trim(),
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Address = request.Address.Trim(),
            Phone = request.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            NationalityCountryId = request.NationalityCountryId,
            ImagePath = string.IsNullOrWhiteSpace(request.ImagePath) ? null : request.ImagePath.Trim(),
        };

        _context.People.Add(person);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Person with id {PersonId} was created successfully.", person.Id);

        await _cache.RemoveByTagAsync("people", cancellationToken);

        return Result<Guid>.Success(person.Id);
    }
}
