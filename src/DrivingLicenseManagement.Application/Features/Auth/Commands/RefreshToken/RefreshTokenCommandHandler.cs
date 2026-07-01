using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Auth.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    ILogger<RefreshTokenCommandHandler> logger,
    IAppDbContext context,
    IIdentityService identityService,
    ITokenProvider tokenProvider)
    : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, ct);

        // IsActive is checked in-memory (not in the SQL predicate) on purpose.
        if (storedToken is null || !storedToken.IsActive)
        {
            _logger.LogWarning("Refresh token is invalid, expired or revoked.");
            return ApplicationErrors.RefreshTokenInvalid;
        }

        var userResult = await _identityService.GetUserByIdAsync(storedToken.UserId, ct);
        if (userResult.IsError)
        {
            _logger.LogWarning("User {UserId} for the refresh token was not found.", storedToken.UserId);
            return userResult.Error!;
        }

        // GenerateJwtTokenAsync revokes the old active token and issues a new pair (rotation).
        var tokenResult = await _tokenProvider.GenerateJwtTokenAsync(userResult.Value, ct);
        if (tokenResult.IsError)
        {
            _logger.LogError(
                "Failed to generate token during refresh for user {UserId}. Error: {ErrorCode} - {ErrorDescription}",
                userResult.Value.Id,
                tokenResult.Error!.Value.Code,
                tokenResult.Error!.Value.Description);
            return tokenResult.Error!;
        }

        return tokenResult.Value;
    }
}
