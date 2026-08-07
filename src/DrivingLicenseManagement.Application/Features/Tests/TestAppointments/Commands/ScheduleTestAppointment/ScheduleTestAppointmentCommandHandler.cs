using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Commands.ScheduleTestAppointment;

public sealed class ScheduleTestAppointmentCommandHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    ILogger<ScheduleTestAppointmentCommandHandler> logger)
    : IRequestHandler<ScheduleTestAppointmentCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<ScheduleTestAppointmentCommandHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(ScheduleTestAppointmentCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Test appointment scheduling attempted with no authenticated user.");

            return Result<Guid>.Failure(ApplicationErrors.UserNotAuthenticated);
        }

        var applicationStatus = await _context.LocalDrivingLicenseApplications
            .Where(ldl => ldl.Id == request.LocalDrivingLicenseApplicationId)
            .Select(ldl => (ApplicationStatus?)ldl.Application.ApplicationStatus)
            .FirstOrDefaultAsync(cancellationToken);

        if (applicationStatus is null)
        {
            _logger.LogWarning(
                "Local driving license application with id {LocalDrivingLicenseApplicationId} was not found.",
                request.LocalDrivingLicenseApplicationId);

            return Result<Guid>.Failure(ApplicationErrors.LocalDrivingLicenseApplicationNotFound);
        }

        if (applicationStatus != ApplicationStatus.New)
        {
            return Result<Guid>.Failure(ApplicationErrors.LocalDrivingLicenseApplicationNotActive);
        }

        var testTypeFees = await _context.TestTypes
            .Where(testType => testType.Id == request.TestTypeId)
            .Select(testType => (decimal?)testType.TestTypeFees)
            .FirstOrDefaultAsync(cancellationToken);

        var order = Array.IndexOf(TestTypeIds.OrderedIds, request.TestTypeId);

        if (testTypeFees is null || order < 0)
        {
            _logger.LogWarning("Test type with id {TestTypeId} was not found.", request.TestTypeId);

            return Result<Guid>.Failure(ApplicationErrors.TestTypeNotFound);
        }

        var passedTestTypeIds = await _context.Tests
            .Where(test => test.TestResult
                && test.TestAppointment.LocalDrivingLicenseApplicationId == request.LocalDrivingLicenseApplicationId)
            .Select(test => test.TestAppointment.TestTypeId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (passedTestTypeIds.Contains(request.TestTypeId))
        {
            return Result<Guid>.Failure(ApplicationErrors.TestTypeAlreadyPassed);
        }

        var previousTestTypeIds = TestTypeIds.OrderedIds.Take(order);

        if (!previousTestTypeIds.All(passedTestTypeIds.Contains))
        {
            return Result<Guid>.Failure(ApplicationErrors.PreviousTestNotPassed);
        }

        // First attempt only: any existing appointment for this test type routes to a retake.
        var existingLockStates = await _context.TestAppointments
            .Where(appointment => appointment.LocalDrivingLicenseApplicationId == request.LocalDrivingLicenseApplicationId
                && appointment.TestTypeId == request.TestTypeId)
            .Select(appointment => appointment.IsLocked)
            .ToListAsync(cancellationToken);

        if (existingLockStates.Any(isLocked => !isLocked))
        {
            return Result<Guid>.Failure(ApplicationErrors.ActiveTestAppointmentExists);
        }

        if (existingLockStates.Count > 0)
        {
            return Result<Guid>.Failure(ApplicationErrors.RetakeRequired);
        }

        var appointment = new TestAppointment
        {
            TestTypeId = request.TestTypeId,
            LocalDrivingLicenseApplicationId = request.LocalDrivingLicenseApplicationId,
            AppointmentDate = request.AppointmentDate,
            PaidFees = testTypeFees.Value, // COPIED — historical value, never re-derived from TestType.
            CreatedByUserId = userId,
            IsLocked = false,
            RetakeTestApplicationId = null,
        };

        _context.TestAppointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Test appointment with id {TestAppointmentId} was scheduled successfully.",
            appointment.Id);

        return Result<Guid>.Success(appointment.Id);
    }
}
