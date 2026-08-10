using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using DomainApplication = DrivingLicenseManagement.Domain.Entities.Application;

namespace DrivingLicenseManagement.Application.Features.DetainedLicenses.Commands.ReleaseDetainedLicense;

public sealed class ReleaseDetainedLicenseCommandHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    ILogger<ReleaseDetainedLicenseCommandHandler> logger,
    HybridCache cache)
    : IRequestHandler<ReleaseDetainedLicenseCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<ReleaseDetainedLicenseCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Guid>> Handle(ReleaseDetainedLicenseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Detained license release attempted with no authenticated user.");

            return Result<Guid>.Failure(ApplicationErrors.UserNotAuthenticated);
        }

        // Loaded tracked because the detention record is updated in place below.
        var detainedLicense = await _context.DetainedLicenses
            .FirstOrDefaultAsync(detainedLicense => detainedLicense.Id == request.DetainedLicenseId, cancellationToken);

        if (detainedLicense is null)
        {
            _logger.LogWarning("Detained license with id {DetainedLicenseId} was not found.", request.DetainedLicenseId);

            return Result<Guid>.Failure(ApplicationErrors.DetainedLicenseNotFound);
        }

        if (detainedLicense.IsReleased)
        {
            return Result<Guid>.Failure(ApplicationErrors.DetainedLicenseAlreadyReleased);
        }

        // Only the LicenseId scalar is on hand, so resolve the applicant's person id via a single join.
        var applicantPersonId = await _context.Licenses
            .Where(license => license.Id == detainedLicense.LicenseId)
            .Select(license => license.Driver.PersonId)
            .FirstOrDefaultAsync(cancellationToken);

        // Release application fee — resolved by fixed id (never by title).
        var releaseApplicationTypeFees = await _context.ApplicationTypes
            .Where(at => at.Id == ApplicationTypeIds.ReleaseDetainedDrivingLicense)
            .Select(at => (decimal?)at.ApplicationTypeFees)
            .FirstOrDefaultAsync(cancellationToken);

        if (releaseApplicationTypeFees is null)
        {
            _logger.LogWarning(
                "Application type with id {ApplicationTypeId} was not found.",
                ApplicationTypeIds.ReleaseDetainedDrivingLicense);

            return Result<Guid>.Failure(ApplicationErrors.ApplicationTypeNotFound);
        }

        var now = DateTime.UtcNow;

        var releaseApplication = new DomainApplication
        {
            ApplicantPersonId = applicantPersonId,
            ApplicationDate = now,
            ApplicationTypeId = ApplicationTypeIds.ReleaseDetainedDrivingLicense,
            ApplicationStatus = ApplicationStatus.Completed,
            LastStatusDate = now,
            PaidFees = releaseApplicationTypeFees.Value, // COPIED — release application fee.
            CreatedByUserId = userId,
        };

        // Update the existing detention in place; no new DetainedLicense row is created,
        detainedLicense.IsReleased = true;
        detainedLicense.ReleaseDate = now;
        detainedLicense.ReleasedByUserId = userId;
        detainedLicense.ReleaseApplication = releaseApplication; // nav set → EF inserts the new Application, one SaveChanges.

        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync("detained-licenses", cancellationToken);

        _logger.LogInformation(
            "Detained license {DetainedLicenseId} was released via application {ApplicationId}.",
            detainedLicense.Id,
            releaseApplication.Id);

        return Result<Guid>.Success(releaseApplication.Id);
    }
}
