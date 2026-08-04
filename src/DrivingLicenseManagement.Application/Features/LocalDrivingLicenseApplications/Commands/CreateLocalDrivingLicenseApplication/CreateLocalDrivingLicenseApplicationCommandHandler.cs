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

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Commands.CreateLocalDrivingLicenseApplication;

public sealed class CreateLocalDrivingLicenseApplicationCommandHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    ILogger<CreateLocalDrivingLicenseApplicationCommandHandler> logger)
    : IRequestHandler<CreateLocalDrivingLicenseApplicationCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<CreateLocalDrivingLicenseApplicationCommandHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(
        CreateLocalDrivingLicenseApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Local driving license application attempted with no authenticated user.");

            return Result<Guid>.Failure(ApplicationErrors.UserNotAuthenticated);
        }

        bool personExists = await _context.People
            .AnyAsync(person => person.Id == request.ApplicantPersonId, cancellationToken);

        if (!personExists)
        {
            _logger.LogWarning("Person with id {PersonId} was not found.", request.ApplicantPersonId);

            return Result<Guid>.Failure(ApplicationErrors.PersonNotFound);
        }

        bool licenseClassExists = await _context.LicenseClasses
            .AnyAsync(licenseClass => licenseClass.Id == request.LicenseClassId, cancellationToken);

        if (!licenseClassExists)
        {
            _logger.LogWarning("License class with id {LicenseClassId} was not found.", request.LicenseClassId);

            return Result<Guid>.Failure(ApplicationErrors.LicenseClassNotFound);
        }

        var applicationType = await _context.ApplicationTypes
            .AsNoTracking()
            .Where(at => at.ApplicationTypeTitle == ApplicationTypeConstants.NewLocalDrivingLicense)
            .Select(at => new { at.Id, at.ApplicationTypeFees })
            .SingleOrDefaultAsync(cancellationToken);

        if (applicationType is null)
        {
            _logger.LogWarning(
                "Application type '{ApplicationTypeTitle}' was not found.",
                ApplicationTypeConstants.NewLocalDrivingLicense);

            return Result<Guid>.Failure(ApplicationErrors.ApplicationTypeNotFound);
        }

        bool hasActiveApplication = await _context.LocalDrivingLicenseApplications
            .AnyAsync(
                ldl => ldl.LicenseClassId == request.LicenseClassId
                    && ldl.Application.ApplicantPersonId == request.ApplicantPersonId
                    && ldl.Application.ApplicationStatus == ApplicationStatus.New,
                cancellationToken);

        if (hasActiveApplication)
        {
            _logger.LogWarning(
                "Person with id {PersonId} already has an active local driving license application for license class {LicenseClassId}.",
                request.ApplicantPersonId,
                request.LicenseClassId);

            return Result<Guid>.Failure(ApplicationErrors.ActiveLocalDrivingLicenseApplicationExists);
        }

        var now = DateTime.UtcNow;

        var application = new DomainApplication
        {
            ApplicantPersonId = request.ApplicantPersonId,
            ApplicationDate = now,
            ApplicationTypeId = applicationType.Id,
            ApplicationStatus = ApplicationStatus.New,
            LastStatusDate = now,
            PaidFees = applicationType.ApplicationTypeFees, // COPIED — historical value, never derived from LicenseClass.ClassFees
            CreatedByUserId = userId,
        };

        var localApplication = new LocalDrivingLicenseApplication
        {
            Application = application, // nav set → EF inserts Application first, both rows in one SaveChanges
            LicenseClassId = request.LicenseClassId,
        };

        _context.LocalDrivingLicenseApplications.Add(localApplication);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Local driving license application with id {LocalDrivingLicenseApplicationId} was created successfully.",
            localApplication.Id);

        return Result<Guid>.Success(localApplication.Id);
    }
}
