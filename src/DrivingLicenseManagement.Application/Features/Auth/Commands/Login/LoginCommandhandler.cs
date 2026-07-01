using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Auth.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(ILogger<LoginCommandHandler> logger, IIdentityService identityService, ITokenProvider tokenProvider)
    : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly ILogger<LoginCommandHandler> _logger = logger;
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var userResponse = await _identityService.AuthenticateAsync(request.Email, request.Password, ct);

        if (userResponse.IsError)
        {
            return userResponse.Error!;
        }

        var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(userResponse.Value, ct);

        if(generateTokenResult.IsError)
        {
            _logger.LogError("Generate token error occurred: {ErrorDescription}", generateTokenResult.Error);

            return generateTokenResult.Error!;
        }

        return generateTokenResult.Value;
    }
}