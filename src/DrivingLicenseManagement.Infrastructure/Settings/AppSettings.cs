using DrivingLicenseManagement.Application.Common.Interfaces;

namespace DrivingLicenseManagement.Infrastructure.Settings;

// Bound from appsettings.json and exposed to the Application layer through IAppSettings.
public sealed class AppSettings : IAppSettings
{
    public InternationalLicenseSettings InternationalLicense { get; set; } = new();
    public ResendSettings Resend { get; set; } = new();

    public string FrontendUrl { get; set; } = string.Empty;



    // Get-only projections the config binder ignores; this is what the Application layer consumes.
    public int InternationalLicenseValidityYears => InternationalLicense.ValidityYears;
    public string ResendFromEmail => Resend.FromEmail;
    public string ResendFromName => Resend.FromName;

}

public sealed class InternationalLicenseSettings
{
    public int ValidityYears { get; set; }
}

public sealed class ResendSettings
{
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}