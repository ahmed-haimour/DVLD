using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.DetainedLicenses.Commands.DetainLicense;

public sealed class DetainLicenseCommandHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    ILogger<DetainLicenseCommandHandler> logger,
    HybridCache cache)
    : IRequestHandler<DetainLicenseCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<DetainLicenseCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Guid>> Handle(DetainLicenseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("License detention attempted with no authenticated user.");

            return Result<Guid>.Failure(ApplicationErrors.UserNotAuthenticated);
        }

        // Read-only: detaining a license never mutates the License row itself.
        var license = await _context.Licenses
            .AsNoTracking()
            .Where(license => license.Id == request.LicenseId)
            .Select(license => new { license.Id, license.IsActive })
            .FirstOrDefaultAsync(cancellationToken);

        if (license is null)
        {
            _logger.LogWarning("License with id {LicenseId} was not found.", request.LicenseId);

            return Result<Guid>.Failure(ApplicationErrors.LicenseNotFound);
        }

        if (!license.IsActive)
        {
            return Result<Guid>.Failure(ApplicationErrors.LicenseNotActive);
        }

        bool hasUnreleasedDetention = await _context.DetainedLicenses
            .AnyAsync(detention => detention.LicenseId == license.Id && !detention.IsReleased, cancellationToken);

        if (hasUnreleasedDetention)
        {
            _logger.LogWarning("License with id {LicenseId} already has an unreleased detention.", license.Id);

            return Result<Guid>.Failure(ApplicationErrors.LicenseAlreadyDetained);
        }

        var detainedLicense = new DetainedLicense
        {
            LicenseId = license.Id,
            DetainDate = DateTime.UtcNow,
            FineFees = request.FineFees,
            CreatedByUserId = userId,
            IsReleased = false,
        };

        _context.DetainedLicenses.Add(detainedLicense);
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync("detained-licenses", cancellationToken);

        _logger.LogInformation(
            "License {LicenseId} was detained with fine fees {FineFees}.",
            license.Id,
            request.FineFees);

        return Result<Guid>.Success(detainedLicense.Id);
    }
}
