using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Resend;

namespace DrivingLicenseManagement.Infrastructure.Services.Email;

public sealed class ResendEmailService(
    IResend resend,
    EmailTemplateRenderer templateRenderer,
    IOptions<AppSettings> options) : IEmailService
{
    private readonly IResend _resend = resend;
    private readonly EmailTemplateRenderer _templateRenderer = templateRenderer;
    private readonly AppSettings _appSettings = options.Value;

    public async Task SendPasswordResetAsync(
        string toEmail,
        string resetUrl,
        CancellationToken cancellationToken = default)
    {
        var htmlBody = await _templateRenderer.RenderAsync(
            "ResetPassword.html",
            new Dictionary<string, string> { ["ResetUrl"] = resetUrl },
            cancellationToken);

        var message = new EmailMessage
        {
            From = $"{_appSettings.ResendFromName} <{_appSettings.ResendFromEmail}>",
            To = toEmail,
            Subject = "Reset your DVLD password",
            HtmlBody = htmlBody
        };

        await _resend.EmailSendAsync(message, cancellationToken);
    }

    public async Task SendEmailVerificationCodeAsync(string toEmail, string verificationCode, CancellationToken cancellationToken = default)
    {
        var htmlBody = await _templateRenderer.RenderAsync("ConfirmEmail.html",
            new Dictionary<string, string>
            {
                ["VerificationCode"] = verificationCode
            }, cancellationToken);

        var message = new EmailMessage
        {
            From = $"{_appSettings.ResendFromName} <{_appSettings.ResendFromEmail}>",
            To = toEmail,
            Subject = "Verify your DVLD email",
            HtmlBody = htmlBody
        };
    
        await _resend.EmailSendAsync(message, cancellationToken);
    }
}
