using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class TestAppointment : Entity
{
    public Guid TestTypeId { get; set; }
    public Guid LocalDrivingLicenseApplicationId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public decimal PaidFees { get; set; }
    public Guid CreatedByUserId { get; set; }
    public bool IsLocked { get; set; }
    public Guid? RetakeTestApplicationId { get; set; }

    public TestType TestType { get; set; } = null!;
    public LocalDrivingLicenseApplication LocalDrivingLicenseApplication { get; set; } = null!;
    public Application? RetakeTestApplication { get; set; }
    public Test? Test { get; set; }
}
