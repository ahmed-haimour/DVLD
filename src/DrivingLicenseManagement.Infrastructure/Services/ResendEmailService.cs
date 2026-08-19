using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Resend;

namespace DrivingLicenseManagement.Infrastructure.Services;

public class ResendEmailService(IResend resend, IOptions<AppSettings> options) : IEmailService
{
    private readonly IResend _resend = resend;
    private readonly AppSettings _appSettings = options.Value;

    public async Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default)
    {
        var message = new EmailMessage
        {
            From = $"{_appSettings.ResendFromName} <{_appSettings.ResendFromEmail}>",
            To = toEmail,
            Subject = subject,
            TextBody = body
        };

        await _resend.EmailSendAsync(message, ct);    
    }
}