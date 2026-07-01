using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(
    ILogger<LogoutCommandHandler> logger,
    IAppDbContext context)
    : IRequestHandler<LogoutCommand, Result>
{
    private readonly ILogger<LogoutCommandHandler> _logger = logger;
    private readonly IAppDbContext _context = context;

    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, ct);

        // Idempotent: an unknown, already-revoked, or already-expired token still counts as a
        // successful logout. IsActive (checked in-memory) covers revoked AND expired in one shot.
        if (storedToken is null || !storedToken.IsActive)
        {
            _logger.LogInformation("Logout for a missing or expired or inactive refresh token; treating as success.");
            return Result.Success();
        }

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Refresh token for user {UserId} was revoked on logout.", storedToken.UserId);

        return Result.Success();
    }
}
