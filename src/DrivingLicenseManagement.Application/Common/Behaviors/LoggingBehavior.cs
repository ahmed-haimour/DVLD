using DrivingLicenseManagement.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest>(
    ILogger<TRequest> logger,
    ICurrentUser currentUser,
    IIdentityService identityService) : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger = logger;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IIdentityService _identityService = identityService;

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.UserId;

        string? userName = null;

        if (userId != Guid.Empty)
        {
            var userResult = await _identityService.GetUserByIdAsync(
                userId,
                cancellationToken);

            if (userResult.IsSuccess)
            {
                userName = userResult.Value.UserName;
            }
        }

        _logger.LogInformation(
            "Request: {RequestName} UserId: {UserId} UserName: {UserName} {@Request}",
            requestName,
            userId,
            userName,
            request);
    }
}