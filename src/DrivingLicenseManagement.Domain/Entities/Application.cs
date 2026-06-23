using DrivingLicenseManagement.Domain.Common;
using DrivingLicenseManagement.Domain.Enums;

namespace DrivingLicenseManagement.Domain.Entities;

public class Application : Entity
{
    public Guid ApplicantPersonId { get; set; }
    public DateTime ApplicationDate { get; set; }
    public Guid ApplicationTypeId { get; set; }
    public ApplicationStatus ApplicationStatus { get; set; }
    public DateTime LastStatusDate { get; set; }
    public decimal PaidFees { get; set; }
    public Guid CreatedByUserId { get; set; }

    public Person ApplicantPerson { get; set; } = null!;
    public ApplicationType ApplicationType { get; set; } = null!;
    public LocalDrivingLicenseApplication? LocalDrivingLicenseApplication { get; set; }
    public License? License { get; set; }
    public DetainedLicense? ReleasedDetainedLicense { get; set; }
    public TestAppointment? RetakeTestAppointment { get; set; }
}
