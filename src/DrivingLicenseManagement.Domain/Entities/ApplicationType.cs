using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class ApplicationType : Entity
{
    public ApplicationType()
    {
    }

    public ApplicationType(Guid id)
        : base(id)
    {
    }

    public string ApplicationTypeTitle { get; set; } = string.Empty;
    public decimal ApplicationTypeFees { get; set; }
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
