using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    ICurrentUser currentUser,
    IIdentityService identityService,
    ILogger<ChangePasswordCommandHandler> logger)
    : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger = logger;

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Password change attempted with no authenticated user.");

            return Result.Failure(ApplicationErrors.UserNotAuthenticated);
        }

        var changeResult = await _identityService
            .ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, ct);

        if (changeResult.IsError)
        {
            _logger.LogWarning(
                "Password change failed for user {UserId}: {ErrorCode}",
                userId,
                changeResult.Error!.Value.Code);

            return changeResult;
        }

        _logger.LogInformation("Password changed for user {UserId}; sessions revoked.", userId);

        return Result.Success();
    }
}
