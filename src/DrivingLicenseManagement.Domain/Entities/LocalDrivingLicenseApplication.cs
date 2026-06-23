using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class LocalDrivingLicenseApplication : Entity
{
    public Guid ApplicationId { get; set; }
    public Guid LicenseClassId { get; set; }

    public Application Application { get; set; } = null!;
    public LicenseClass LicenseClass { get; set; } = null!;
    public ICollection<TestAppointment> TestAppointments { get; set; } = new List<TestAppointment>();
}
