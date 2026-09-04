using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using System.Buffers.Text;
using System.Text;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IIdentityService identityService,
    IEmailService emailService,
    IAppSettings appSettings)
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IEmailService _emailService = emailService;
    private readonly IAppSettings _appSettings = appSettings;

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var token = await _identityService
            .GeneratePasswordResetTokenAsync(request.Email, ct);

        // Do not reveal whether the email exists.
        if (token is null)
        {
            return Result.Success();
        }

        var encodedEmail = Uri.EscapeDataString(request.Email);
        var encodedToken = Base64Url.EncodeToString(Encoding.UTF8.GetBytes(token));

        var resetUrl =
            $"{_appSettings.FrontendUrl}/reset-password" +
            $"?email={encodedEmail}&token={encodedToken}";

        await _emailService.SendPasswordResetAsync(
            request.Email,
            resetUrl,
            ct);

        return Result.Success();
    }
}
