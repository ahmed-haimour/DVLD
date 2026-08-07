using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.Tests.Tests.Commands.TakeTest;

public sealed class TakeTestCommandHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    ILogger<TakeTestCommandHandler> logger)
    : IRequestHandler<TakeTestCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<TakeTestCommandHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(TakeTestCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Take test attempted with no authenticated user.");

            return Result<Guid>.Failure(ApplicationErrors.UserNotAuthenticated);
        }

        var appointment = await _context.TestAppointments
            .FirstOrDefaultAsync(appointment => appointment.Id == request.TestAppointmentId, cancellationToken);

        if (appointment is null)
        {
            _logger.LogWarning("Test appointment with id {TestAppointmentId} was not found.", request.TestAppointmentId);

            return Result<Guid>.Failure(ApplicationErrors.TestAppointmentNotFound);
        }

        // A locked appointment already has a test recorded, so this also blocks a double-take.
        if (appointment.IsLocked)
        {
            return Result<Guid>.Failure(ApplicationErrors.TestAppointmentLocked);
        }

        var applicationStatus = await _context.LocalDrivingLicenseApplications
            .Where(ldl => ldl.Id == appointment.LocalDrivingLicenseApplicationId)
            .Select(ldl => (ApplicationStatus?)ldl.Application.ApplicationStatus)
            .FirstOrDefaultAsync(cancellationToken);

        if (applicationStatus != ApplicationStatus.New)
        {
            return Result<Guid>.Failure(ApplicationErrors.LocalDrivingLicenseApplicationNotActive);
        }

        var test = new Test
        {
            TestAppointmentId = appointment.Id,
            TestResult = request.TestResult,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            CreatedByUserId = userId,
        };

        // Locking happens whether the applicant passed or failed; the appointment is now historical.
        appointment.IsLocked = true;

        _context.Tests.Add(test);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Test with id {TestId} was recorded for appointment {TestAppointmentId} (result: {TestResult}).",
            test.Id,
            appointment.Id,
            request.TestResult);

        return Result<Guid>.Success(test.Id);
    }
}
