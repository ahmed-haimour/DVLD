using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class ApplicationType : Entity
{
    public string ApplicationTypeTitle { get; set; } = string.Empty;
    public decimal ApplicationTypeFees { get; set; }
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
