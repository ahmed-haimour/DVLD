using DrivingLicenseManagement.Application.Common.Interfaces;

namespace DrivingLicenseManagement.Infrastructure.Settings;

// Bound from appsettings.json and exposed to the Application layer through IAppSettings.
public sealed class AppSettings : IAppSettings
{
    public InternationalLicenseSettings InternationalLicense { get; set; } = new();

    // Get-only projections the config binder ignores; this is what the Application layer consumes.
    public int InternationalLicenseValidityYears => InternationalLicense.ValidityYears;
}

public sealed class InternationalLicenseSettings
{
    public int ValidityYears { get; set; }
}
