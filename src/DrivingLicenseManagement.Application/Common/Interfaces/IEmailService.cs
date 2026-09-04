namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetAsync(
        string toEmail,
        string resetUrl,
        CancellationToken cancellationToken = default);
}
