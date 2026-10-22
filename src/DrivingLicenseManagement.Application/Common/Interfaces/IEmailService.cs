namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetAsync(
        string toEmail,
        string resetUrl,
        CancellationToken cancellationToken = default);

    Task SendEmailVerificationCodeAsync(
        string toEmail,
        string verificationCode,
        CancellationToken cancellationToken = default);
}
