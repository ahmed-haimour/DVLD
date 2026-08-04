using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Commands.CancelLocalDrivingLicenseApplication;

public sealed class CancelLocalDrivingLicenseApplicationCommandHandler(
    IAppDbContext context,
    ILogger<CancelLocalDrivingLicenseApplicationCommandHandler> logger)
    : IRequestHandler<CancelLocalDrivingLicenseApplicationCommand, Result>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CancelLocalDrivingLicenseApplicationCommandHandler> _logger = logger;

    public async Task<Result> Handle(
        CancelLocalDrivingLicenseApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var localApplication = await _context.LocalDrivingLicenseApplications
            .Include(ldl => ldl.Application)
            .FirstOrDefaultAsync(
                ldl => ldl.Id == request.LocalDrivingLicenseApplicationId,
                cancellationToken);

        if (localApplication is null)
        {
            _logger.LogWarning(
                "Local driving license application with id {LocalDrivingLicenseApplicationId} was not found.",
                request.LocalDrivingLicenseApplicationId);

            return Result.Failure(ApplicationErrors.LocalDrivingLicenseApplicationNotFound);
        }

        var application = localApplication.Application;

        if (application.ApplicationStatus != ApplicationStatus.New)
        {
            _logger.LogWarning(
                "Local driving license application with id {LocalDrivingLicenseApplicationId} cannot be cancelled because its status is {ApplicationStatus}.",
                request.LocalDrivingLicenseApplicationId,
                application.ApplicationStatus);

            return Result.Failure(ApplicationErrors.LocalDrivingLicenseApplicationCannotBeCancelled);
        }

        application.ApplicationStatus = ApplicationStatus.Cancelled;
        application.LastStatusDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Local driving license application with id {LocalDrivingLicenseApplicationId} was cancelled successfully.",
            request.LocalDrivingLicenseApplicationId);

        return Result.Success();
    }
}
