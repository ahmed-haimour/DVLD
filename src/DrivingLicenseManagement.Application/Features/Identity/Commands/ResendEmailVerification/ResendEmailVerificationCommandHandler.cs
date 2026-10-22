using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.ResendEmailVerification;

public sealed class ResendEmailVerificationCommandHandler(IIdentityService identityService, IEmailService emailService, ILogger<ResendEmailVerificationCommandHandler> logger)
    : IRequestHandler<ResendEmailVerificationCommand, Result>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<ResendEmailVerificationCommandHandler> _logger = logger;

    public async Task<Result> Handle(ResendEmailVerificationCommand request, CancellationToken ct)
    {
        var codeResult = await _identityService.ResendEmailVerificationCodeAsync(request.Email, ct);

        if (codeResult.IsError)
        {
            return Result.Failure(codeResult.Error!.Value);
        }

        await _emailService.SendEmailVerificationCodeAsync(request.Email, codeResult.Value, ct);

        _logger.LogInformation("Email verification code resent to {MaskedEmail}.", UtilityService.MaskEmail(request.Email));

        return Result.Success();
    }
}