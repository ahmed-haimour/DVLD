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

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Commands.ScheduleRetakeTestAppointment;

public sealed class ScheduleRetakeTestAppointmentCommandHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    ILogger<ScheduleRetakeTestAppointmentCommandHandler> logger)
    : IRequestHandler<ScheduleRetakeTestAppointmentCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<ScheduleRetakeTestAppointmentCommandHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(ScheduleRetakeTestAppointmentCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Retake test appointment scheduling attempted with no authenticated user.");

            return Result<Guid>.Failure(ApplicationErrors.UserNotAuthenticated);
        }

        var application = await _context.LocalDrivingLicenseApplications
            .Where(ldl => ldl.Id == request.LocalDrivingLicenseApplicationId)
            .Select(ldl => new
            {
                ldl.Application.ApplicationStatus,
                ldl.Application.ApplicantPersonId,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (application is null)
        {
            _logger.LogWarning(
                "Local driving license application with id {LocalDrivingLicenseApplicationId} was not found.",
                request.LocalDrivingLicenseApplicationId);

            return Result<Guid>.Failure(ApplicationErrors.LocalDrivingLicenseApplicationNotFound);
        }

        if (application.ApplicationStatus != ApplicationStatus.New)
        {
            return Result<Guid>.Failure(ApplicationErrors.LocalDrivingLicenseApplicationNotActive);
        }

        var testTypeFees = await _context.TestTypes
            .Where(testType => testType.Id == request.TestTypeId)
            .Select(testType => (decimal?)testType.TestTypeFees)
            .FirstOrDefaultAsync(cancellationToken);

        if (testTypeFees is null)
        {
            _logger.LogWarning("Test type with id {TestTypeId} was not found.", request.TestTypeId);

            return Result<Guid>.Failure(ApplicationErrors.TestTypeNotFound);
        }

        bool hasActiveAppointment = await _context.TestAppointments
            .AnyAsync(appointment => appointment.LocalDrivingLicenseApplicationId == request.LocalDrivingLicenseApplicationId
                && appointment.TestTypeId == request.TestTypeId
                && !appointment.IsLocked,
                cancellationToken);

        if (hasActiveAppointment)
        {
            return Result<Guid>.Failure(ApplicationErrors.ActiveTestAppointmentExists);
        }

        bool alreadyPassed = await _context.Tests
            .AnyAsync(test => test.TestResult
                && test.TestAppointment.LocalDrivingLicenseApplicationId == request.LocalDrivingLicenseApplicationId
                && test.TestAppointment.TestTypeId == request.TestTypeId,
                cancellationToken);

        if (alreadyPassed)
        {
            return Result<Guid>.Failure(ApplicationErrors.TestTypeAlreadyPassed);
        }

        // The latest attempt for this test type must be a locked, failed test to justify a retake.
        var latestAppointment = await _context.TestAppointments
            .Where(appointment => appointment.LocalDrivingLicenseApplicationId == request.LocalDrivingLicenseApplicationId
                && appointment.TestTypeId == request.TestTypeId)
            .OrderByDescending(appointment => appointment.AppointmentDate)
            .Select(appointment => new
            {
                appointment.IsLocked,
                TestResult = appointment.Test != null ? appointment.Test.TestResult : (bool?)null,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (latestAppointment is null || !latestAppointment.IsLocked || latestAppointment.TestResult != false)
        {
            return Result<Guid>.Failure(ApplicationErrors.NoFailedTestToRetake);
        }

        // Resolved by fixed id (never by title — administrators may rename application types).
        var retakeApplicationTypeFees = await _context.ApplicationTypes
            .Where(at => at.Id == ApplicationTypeIds.RetakeTest)
            .Select(at => (decimal?)at.ApplicationTypeFees)
            .FirstOrDefaultAsync(cancellationToken);

        if (retakeApplicationTypeFees is null)
        {
            _logger.LogWarning(
                "Application type with id {ApplicationTypeId} was not found.",
                ApplicationTypeIds.RetakeTest);

            return Result<Guid>.Failure(ApplicationErrors.ApplicationTypeNotFound);
        }

        var now = DateTime.UtcNow;

        var retakeApplication = new DomainApplication
        {
            ApplicantPersonId = application.ApplicantPersonId,
            ApplicationDate = now,
            ApplicationTypeId = ApplicationTypeIds.RetakeTest,
            ApplicationStatus = ApplicationStatus.New,
            LastStatusDate = now,
            PaidFees = retakeApplicationTypeFees.Value, // COPIED — retake application fee.
            CreatedByUserId = userId,
        };

        var appointment = new TestAppointment
        {
            TestTypeId = request.TestTypeId,
            LocalDrivingLicenseApplicationId = request.LocalDrivingLicenseApplicationId,
            AppointmentDate = request.AppointmentDate,
            PaidFees = testTypeFees.Value, // COPIED — test appointment fee.
            CreatedByUserId = userId,
            IsLocked = false,
            RetakeTestApplication = retakeApplication, // nav set → EF inserts the retake Application first, one SaveChanges.
        };

        _context.TestAppointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Retake test appointment with id {TestAppointmentId} was scheduled successfully (retake application {ApplicationId}).",
            appointment.Id,
            retakeApplication.Id);

        return Result<Guid>.Success(appointment.Id);
    }
}
