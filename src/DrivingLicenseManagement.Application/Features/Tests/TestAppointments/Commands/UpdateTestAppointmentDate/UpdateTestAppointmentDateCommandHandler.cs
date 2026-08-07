using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Commands.UpdateTestAppointmentDate;

public sealed class UpdateTestAppointmentDateCommandHandler(
    IAppDbContext context,
    ILogger<UpdateTestAppointmentDateCommandHandler> logger)
    : IRequestHandler<UpdateTestAppointmentDateCommand, Result>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateTestAppointmentDateCommandHandler> _logger = logger;

    public async Task<Result> Handle(UpdateTestAppointmentDateCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.TestAppointments
            .FirstOrDefaultAsync(appointment => appointment.Id == request.TestAppointmentId, cancellationToken);

        if (appointment is null)
        {
            _logger.LogWarning("Test appointment with id {TestAppointmentId} was not found.", request.TestAppointmentId);

            return Result.Failure(ApplicationErrors.TestAppointmentNotFound);
        }

        if (appointment.IsLocked)
        {
            return Result.Failure(ApplicationErrors.TestAppointmentLocked);
        }

        // AppointmentDate is the only editable field.
        appointment.AppointmentDate = request.AppointmentDate;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Test appointment with id {TestAppointmentId} date was updated successfully.",
            appointment.Id);

        return Result.Success();
    }
}
