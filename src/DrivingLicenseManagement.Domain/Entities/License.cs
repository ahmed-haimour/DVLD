using DrivingLicenseManagement.Domain.Common;
using DrivingLicenseManagement.Domain.Enums;
namespace DrivingLicenseManagement.Domain.Entities;

public class License : Entity
{
    public Guid ApplicationId { get; set; }
    public Guid DriverId { get; set; }
    public Guid LicenseClassId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string? Notes { get; set; }
    public decimal PaidFees { get; set; }
    public bool IsActive { get; set; }
    public LicenseIssueReason IssueReason { get; set; }
    public Guid CreatedByUserId { get; set; }

    public Application Application { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
    public LicenseClass LicenseClass { get; set; } = null!;
    public ICollection<DetainedLicense> Detentions { get; set; } = new List<DetainedLicense>();
}
