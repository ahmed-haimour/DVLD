using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class Test : Entity
{
    public Guid TestAppointmentId { get; set; }
    public bool TestResult { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedByUserId { get; set; }

    public TestAppointment TestAppointment { get; set; } = null!;
}
