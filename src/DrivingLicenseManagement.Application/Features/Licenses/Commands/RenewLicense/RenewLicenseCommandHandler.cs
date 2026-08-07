using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using DomainApplication = DrivingLicenseManagement.Domain.Entities.Application;

namespace DrivingLicenseManagement.Application.Features.Licenses.Commands.RenewLicense;

public sealed class RenewLicenseCommandHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    ILogger<RenewLicenseCommandHandler> logger,
    HybridCache cache)
    : IRequestHandler<RenewLicenseCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<RenewLicenseCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Guid>> Handle(RenewLicenseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("License renewal attempted with no authenticated user.");

            return Result<Guid>.Failure(ApplicationErrors.UserNotAuthenticated);
        }

        // Loaded tracked because the old license is deactivated below.
        var license = await _context.Licenses
            .FirstOrDefaultAsync(license => license.Id == request.LicenseId, cancellationToken);

        if (license is null)
        {
            _logger.LogWarning("License with id {LicenseId} was not found.", request.LicenseId);

            return Result<Guid>.Failure(ApplicationErrors.LicenseNotFound);
        }

        if (!license.IsActive)
        {
            return Result<Guid>.Failure(ApplicationErrors.LicenseNotActive);
        }

        var now = DateTime.UtcNow;

        if (license.ExpirationDate.Date >= now.Date)
        {
            return Result<Guid>.Failure(ApplicationErrors.LicenseNotExpired);
        }

        // A driver may hold licenses for other classes, but never two active for the SAME class.
        bool anotherActiveLicense = await _context.Licenses
            .AnyAsync(other => other.Id != license.Id
                && other.DriverId == license.DriverId
                && other.LicenseClassId == license.LicenseClassId
                && other.IsActive,
                cancellationToken);

        if (anotherActiveLicense)
        {
            _logger.LogWarning(
                "Driver {DriverId} already has another active license for license class {LicenseClassId}.",
                license.DriverId,
                license.LicenseClassId);

            return Result<Guid>.Failure(ApplicationErrors.ActiveLicenseAlreadyExists);
        }

        // The driver already exists (the license references it) and is reused — never created here.
        var applicantPersonId = await _context.Drivers
            .Where(driver => driver.Id == license.DriverId)
            .Select(driver => driver.PersonId)
            .FirstOrDefaultAsync(cancellationToken);

        // Renew application fee — resolved by fixed id (never by title).
        var renewApplicationTypeFees = await _context.ApplicationTypes
            .Where(at => at.Id == ApplicationTypeIds.RenewLicense)
            .Select(at => (decimal?)at.ApplicationTypeFees)
            .FirstOrDefaultAsync(cancellationToken);

        if (renewApplicationTypeFees is null)
        {
            _logger.LogWarning(
                "Application type with id {ApplicationTypeId} was not found.",
                ApplicationTypeIds.RenewLicense);

            return Result<Guid>.Failure(ApplicationErrors.ApplicationTypeNotFound);
        }

        var licenseClass = await _context.LicenseClasses
            .Where(lc => lc.Id == license.LicenseClassId)
            .Select(lc => new { lc.ClassFees, lc.DefaultValidityLength })
            .FirstOrDefaultAsync(cancellationToken);

        if (licenseClass is null)
        {
            _logger.LogWarning("License class with id {LicenseClassId} was not found.", license.LicenseClassId);

            return Result<Guid>.Failure(ApplicationErrors.LicenseClassNotFound);
        }

        var renewApplication = new DomainApplication
        {
            ApplicantPersonId = applicantPersonId,
            ApplicationDate = now,
            ApplicationTypeId = ApplicationTypeIds.RenewLicense,
            ApplicationStatus = ApplicationStatus.Completed,
            LastStatusDate = now,
            PaidFees = renewApplicationTypeFees.Value, // COPIED — renew application fee.
            CreatedByUserId = userId,
        };

        var newLicense = new License
        {
            Application = renewApplication, // nav set → EF inserts the new Application first (1:1), one SaveChanges.
            DriverId = license.DriverId, // reuse the existing driver.
            LicenseClassId = license.LicenseClassId,
            IssueDate = now,
            ExpirationDate = now.AddYears(licenseClass.DefaultValidityLength),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            PaidFees = licenseClass.ClassFees, // COPIED — historical value.
            IsActive = true,
            IssueReason = LicenseIssueReason.Renew,
            CreatedByUserId = userId,
        };

        // Deactivate the previous license; all other historical data is preserved.
        license.IsActive = false;

        _context.Licenses.Add(newLicense);
        await _context.SaveChangesAsync(cancellationToken);

        // The active-license count for this driver changed.
        await _cache.RemoveByTagAsync("drivers", cancellationToken);
        await _cache.RemoveByTagAsync("licenses", cancellationToken);

        _logger.LogInformation(
            "License {OldLicenseId} was renewed; new license {NewLicenseId} issued.",
            license.Id,
            newLicense.Id);

        return Result<Guid>.Success(newLicense.Id);
    }
}
