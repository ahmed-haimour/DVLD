using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler(IIdentityService identityService, ILogger<VerifyEmailCommandHandler> logger)
    : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<VerifyEmailCommandHandler> _logger = logger;

    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken ct)
    {
        var result = await _identityService.VerifyEmailAsync(request.Email, request.Code, ct);

        if (result.IsSuccess)
            _logger.LogInformation("Email verification completed successfully.");

        return result;
    }
}
