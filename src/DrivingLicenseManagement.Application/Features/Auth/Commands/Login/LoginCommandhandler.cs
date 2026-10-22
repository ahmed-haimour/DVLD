using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Auth.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(ILogger<LoginCommandHandler> logger, IIdentityService identityService, ITokenProvider tokenProvider, IEmailService emailService)
    : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    private readonly ILogger<LoginCommandHandler> _logger = logger;
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken ct)
    {
        var userResponse = await _identityService.AuthenticateAsync(
            request.Email,
            request.Password,
            ct);

        if (userResponse.IsError)
        {
            return userResponse.Error!;
        }

        var user = userResponse.Value;

        if (!user.EmailConfirmed)
        {
            var codeResult = await _identityService.EnsureEmailVerificationCodeAsync(user.Id, ct);

            if (codeResult.IsError)
            {
                return codeResult.Error!;
            }

            if (codeResult.Value is not null)
            {
                await _emailService.SendEmailVerificationCodeAsync(
                    user.Email,
                    codeResult.Value,
                    ct);
            }

            return new LoginResult(
                RequiresEmailVerification: true,
                Tokens: null);
        }

        var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(user, ct);

        if (generateTokenResult.IsError)
        {
            _logger.LogError("Generate token error occurred: {ErrorDescription}", generateTokenResult.Error);
            return generateTokenResult.Error!;
        }

        return new LoginResult(
            RequiresEmailVerification: false,
            Tokens: generateTokenResult.Value);
    }
}