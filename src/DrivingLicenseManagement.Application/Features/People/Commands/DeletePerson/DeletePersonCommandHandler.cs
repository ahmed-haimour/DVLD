using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.People.Commands.DeletePerson;

public sealed class DeletePersonCommandHandler(
    IAppDbContext context,
    IIdentityService identityService,
    HybridCache cache,
    ILogger<DeletePersonCommandHandler> logger)
    : IRequestHandler<DeletePersonCommand, Result>
{
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;
    private readonly HybridCache _cache = cache;
    private readonly ILogger<DeletePersonCommandHandler> _logger = logger;

    public async Task<Result> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.People
            .FirstOrDefaultAsync(person => person.Id == request.Id, cancellationToken);

        if (person is null)
        {
            _logger.LogWarning("Person with id {PersonId} was not found.", request.Id);
            return Result.Failure(ApplicationErrors.PersonNotFound);
        }

        bool hasRelatedRecords = await _context.Drivers
            .AnyAsync(driver => driver.PersonId == request.Id, cancellationToken);

        if (!hasRelatedRecords)
        {
            hasRelatedRecords = await _context.Applications
                .AnyAsync(application => application.ApplicantPersonId == request.Id, cancellationToken);
        }

        if (!hasRelatedRecords)
        {
            hasRelatedRecords = await _identityService
                .PersonHasUserAccountAsync(request.Id, cancellationToken);
        }

        if (hasRelatedRecords)
        {
            _logger.LogWarning("Person with id {PersonId} cannot be deleted because related records exist.", request.Id);
            return Result.Failure(ApplicationErrors.PersonCannotBeDeleted);
        }

        _context.People.Remove(person);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Person with id {PersonId} was deleted successfully.", person.Id);
        
        await _cache.RemoveByTagAsync("people", cancellationToken);

        return Result.Success();
    }
}
