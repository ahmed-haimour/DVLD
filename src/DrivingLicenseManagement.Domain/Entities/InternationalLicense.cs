using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class InternationalLicense : Entity
{
    public Guid ApplicationId { get; set; }
    public Guid DriverId { get; set; }
    public Guid IssuedUsingLocalLicenseId { get; set; }

    public DateTime IssueDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedByUserId { get; set; }

    public Application Application { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
    public License IssuedUsingLocalLicense { get; set; } = null!;
}
