using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class LicenseClass : Entity
{
    public string ClassName { get; set; } = string.Empty;
    public string ClassDescription { get; set; } = string.Empty;
    public byte MinimumAllowedAge { get; set; }
    public byte DefaultValidityLength { get; set; }
    public decimal ClassFees { get; set; }

    public ICollection<LocalDrivingLicenseApplication> LocalDrivingLicenseApplications { get; set; } = new List<LocalDrivingLicenseApplication>();
    public ICollection<License> Licenses { get; set; } = new List<License>();
}
