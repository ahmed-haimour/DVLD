namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface IAppSettings
{
    int InternationalLicenseValidityYears { get; }

    string FrontendUrl { get; }
}
