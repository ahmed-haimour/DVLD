using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DomainApplication = DrivingLicenseManagement.Domain.Entities.Application;

namespace DrivingLicenseManagement.Application.Features.Licenses.Commands.IssueInternationalLicense;

public sealed class IssueInternationalLicenseCommandHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    IAppSettings appSettings,
    ILogger<IssueInternationalLicenseCommandHandler> logger)
    : IRequestHandler<IssueInternationalLicenseCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAppSettings _appSettings = appSettings;
    private readonly ILogger<IssueInternationalLicenseCommandHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(IssueInternationalLicenseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("International license issuance attempted with no authenticated user.");

            return Result<Guid>.Failure(ApplicationErrors.UserNotAuthenticated);
        }

        // The local license is only read (never mutated), so project the scalars we need.
        var localLicense = await _context.Licenses
            .Where(license => license.Id == request.LicenseId)
            .Select(license => new
            {
                license.DriverId,
                license.LicenseClassId,
                license.IsActive,
                license.ExpirationDate,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (localLicense is null)
        {
            _logger.LogWarning("License with id {LicenseId} was not found.", request.LicenseId);

            return Result<Guid>.Failure(ApplicationErrors.LicenseNotFound);
        }

        if (!localLicense.IsActive)
        {
            return Result<Guid>.Failure(ApplicationErrors.LicenseNotActive);
        }

        // Active and expired are different concepts: an active-but-expired license is not eligible.
        if (localLicense.ExpirationDate.Date < DateTime.UtcNow.Date)
        {
            return Result<Guid>.Failure(ApplicationErrors.LicenseExpired);
        }

        // Eligibility is by the fixed Class 3 id, never by (editable) title.
        if (localLicense.LicenseClassId != LicenseClassIds.OrdinaryDrivingLicense)
        {
            return Result<Guid>.Failure(ApplicationErrors.LicenseNotEligibleForInternational);
        }

        bool hasActiveInternationalLicense = await _context.InternationalLicenses
            .AnyAsync(il => il.DriverId == localLicense.DriverId && il.IsActive, cancellationToken);

        if (hasActiveInternationalLicense)
        {
            _logger.LogWarning(
                "Driver {DriverId} already has an active international license.",
                localLicense.DriverId);

            return Result<Guid>.Failure(ApplicationErrors.ActiveInternationalLicenseExists);
        }

        // International-license application fee — resolved by fixed id (never by title).
        var applicationTypeFees = await _context.ApplicationTypes
            .Where(at => at.Id == ApplicationTypeIds.NewInternationalLicense)
            .Select(at => (decimal?)at.ApplicationTypeFees)
            .FirstOrDefaultAsync(cancellationToken);

        if (applicationTypeFees is null)
        {
            _logger.LogWarning(
                "Application type with id {ApplicationTypeId} was not found.",
                ApplicationTypeIds.NewInternationalLicense);

            return Result<Guid>.Failure(ApplicationErrors.ApplicationTypeNotFound);
        }

        var applicantPersonId = await _context.Drivers
            .Where(driver => driver.Id == localLicense.DriverId)
            .Select(driver => driver.PersonId)
            .FirstOrDefaultAsync(cancellationToken);

        var now = DateTime.UtcNow;

        var application = new DomainApplication
        {
            ApplicantPersonId = applicantPersonId,
            ApplicationDate = now,
            ApplicationTypeId = ApplicationTypeIds.NewInternationalLicense,
            ApplicationStatus = ApplicationStatus.Completed,
            LastStatusDate = now,
            PaidFees = applicationTypeFees.Value, // COPIED — international application fee.
            CreatedByUserId = userId,
        };

        var internationalLicense = new InternationalLicense
        {
            Application = application, // nav set → EF inserts the new Application first (1:0..1), one SaveChanges.
            DriverId = localLicense.DriverId,
            IssuedUsingLocalLicenseId = request.LicenseId,
            IssueDate = now,
            ExpirationDate = now.AddYears(_appSettings.InternationalLicenseValidityYears),
            IsActive = true,
            CreatedByUserId = userId,
        };

        _context.InternationalLicenses.Add(internationalLicense);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "International license {InternationalLicenseId} was issued from local license {LicenseId}.",
            internationalLicense.Id,
            request.LicenseId);

        return Result<Guid>.Success(internationalLicense.Id);
    }
}
