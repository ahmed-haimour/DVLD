using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class Driver : Entity
{
    public Guid PersonId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public Person Person { get; set; } = null!;
    public ICollection<License> Licenses { get; set; } = new List<License>();
}
