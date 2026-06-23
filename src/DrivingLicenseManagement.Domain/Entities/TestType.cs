using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class TestType : Entity
{
    public string TestTypeTitle { get; set; } = string.Empty;
    public string TestTypeDescription { get; set; } = string.Empty;
    public decimal TestTypeFees { get; set; }
    public ICollection<TestAppointment> TestAppointments { get; set; } = new List<TestAppointment>();
}
