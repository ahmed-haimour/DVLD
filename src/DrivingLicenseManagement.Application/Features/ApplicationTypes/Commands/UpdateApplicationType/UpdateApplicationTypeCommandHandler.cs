using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.ApplicationTypes.Commands.UpdateApplicationType;

public sealed class UpdateApplicationTypeCommandHandler(
    IAppDbContext context,
    ILogger<UpdateApplicationTypeCommandHandler> logger,
    HybridCache cache)
    : IRequestHandler<UpdateApplicationTypeCommand, Result>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateApplicationTypeCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result> Handle(UpdateApplicationTypeCommand request, CancellationToken cancellationToken)
    {
        var applicationType = await _context.ApplicationTypes
            .FirstOrDefaultAsync(applicationType => applicationType.Id == request.Id, cancellationToken);

        if (applicationType is null)
        {
            _logger.LogWarning("Application type with id {ApplicationTypeId} was not found.", request.Id);

            return Result.Failure(ApplicationErrors.ApplicationTypeNotFound);
        }

        applicationType.ApplicationTypeTitle = request.ApplicationTypeTitle.Trim();
        applicationType.ApplicationTypeFees = request.ApplicationTypeFees;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Application type with id {ApplicationTypeId} was updated successfully.", applicationType.Id);

        await _cache.RemoveByTagAsync("application-types", cancellationToken);

        return Result.Success();
    }
}
