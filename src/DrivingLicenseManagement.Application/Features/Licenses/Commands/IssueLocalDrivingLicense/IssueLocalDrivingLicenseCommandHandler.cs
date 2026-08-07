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

namespace DrivingLicenseManagement.Application.Features.Licenses.Commands.IssueLocalDrivingLicense;

public sealed class IssueLocalDrivingLicenseCommandHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    ILogger<IssueLocalDrivingLicenseCommandHandler> logger,
    HybridCache cache)
    : IRequestHandler<IssueLocalDrivingLicenseCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<IssueLocalDrivingLicenseCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Guid>> Handle(IssueLocalDrivingLicenseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("License issuance attempted with no authenticated user.");

            return Result<Guid>.Failure(ApplicationErrors.UserNotAuthenticated);
        }

        // The Application is loaded tracked because it is completed (mutated) below.
        var localApplication = await _context.LocalDrivingLicenseApplications
            .Include(ldl => ldl.Application)
            .FirstOrDefaultAsync(ldl => ldl.Id == request.LocalDrivingLicenseApplicationId, cancellationToken);

        if (localApplication is null)
        {
            _logger.LogWarning(
                "Local driving license application with id {LocalDrivingLicenseApplicationId} was not found.",
                request.LocalDrivingLicenseApplicationId);

            return Result<Guid>.Failure(ApplicationErrors.LocalDrivingLicenseApplicationNotFound);
        }

        if (localApplication.Application.ApplicationStatus != ApplicationStatus.New)
        {
            return Result<Guid>.Failure(ApplicationErrors.LocalDrivingLicenseApplicationNotActive);
        }

        var applicantPersonId = localApplication.Application.ApplicantPersonId;

        var passedTestTypeIds = await _context.Tests
            .Where(test => test.TestResult
                && test.TestAppointment.LocalDrivingLicenseApplicationId == request.LocalDrivingLicenseApplicationId)
            .Select(test => test.TestAppointment.TestTypeId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!TestTypeIds.OrderedIds.All(passedTestTypeIds.Contains))
        {
            return Result<Guid>.Failure(ApplicationErrors.RequiredTestsNotPassed);
        }

        var now = DateTime.UtcNow;

        var driver = await _context.Drivers
            .FirstOrDefaultAsync(driver => driver.PersonId == applicantPersonId, cancellationToken);

        bool driverIsNew = driver is null;

        if (driver is null)
        {
            driver = new Driver
            {
                PersonId = applicantPersonId,
                CreatedDate = now,
                CreatedByUserId = userId,
            };
        }
        else
        {
            // Active licenses for OTHER classes are valid; only reject a duplicate for THIS class.
            bool hasActiveLicense = await _context.Licenses
                .AnyAsync(license => license.DriverId == driver.Id
                    && license.LicenseClassId == localApplication.LicenseClassId
                    && license.IsActive,
                    cancellationToken);

            if (hasActiveLicense)
            {
                _logger.LogWarning(
                    "Driver {DriverId} already has an active license for license class {LicenseClassId}.",
                    driver.Id,
                    localApplication.LicenseClassId);

                return Result<Guid>.Failure(ApplicationErrors.ActiveLicenseAlreadyExists);
            }
        }

        var licenseClass = await _context.LicenseClasses
            .Where(lc => lc.Id == localApplication.LicenseClassId)
            .Select(lc => new { lc.ClassFees, lc.DefaultValidityLength })
            .FirstOrDefaultAsync(cancellationToken);

        if (licenseClass is null)
        {
            _logger.LogWarning("License class with id {LicenseClassId} was not found.", localApplication.LicenseClassId);

            return Result<Guid>.Failure(ApplicationErrors.LicenseClassNotFound);
        }

        var license = new License
        {
            ApplicationId = localApplication.ApplicationId,
            Driver = driver, // nav set → EF inserts the new driver first if needed, one SaveChanges.
            LicenseClassId = localApplication.LicenseClassId,
            IssueDate = now,
            ExpirationDate = now.AddYears(licenseClass.DefaultValidityLength),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            PaidFees = licenseClass.ClassFees, // COPIED — historical value.
            IsActive = true,
            IssueReason = LicenseIssueReason.FirstTime,
            CreatedByUserId = userId,
        };

        if (driverIsNew)
        {
            _context.Drivers.Add(driver);
        }

        _context.Licenses.Add(license);

        // Complete the original application.
        localApplication.Application.ApplicationStatus = ApplicationStatus.Completed;
        localApplication.Application.LastStatusDate = now;

        await _context.SaveChangesAsync(cancellationToken);

        // A driver may have been created and/or the driver's active-license count changed.
        await _cache.RemoveByTagAsync("drivers", cancellationToken);
        await _cache.RemoveByTagAsync("licenses", cancellationToken);

        _logger.LogInformation(
            "License with id {LicenseId} was issued for local driving license application {LocalDrivingLicenseApplicationId}.",
            license.Id,
            request.LocalDrivingLicenseApplicationId);

        return Result<Guid>.Success(license.Id);
    }
}
