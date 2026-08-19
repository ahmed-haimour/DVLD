namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}