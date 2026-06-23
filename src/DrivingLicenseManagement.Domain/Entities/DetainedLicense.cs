using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class DetainedLicense : Entity
{
    public Guid LicenseId { get; set; }
    public DateTime DetainDate { get; set; }
    public decimal FineFees { get; set; }
    public Guid CreatedByUserId { get; set; }
    public bool IsReleased { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public Guid? ReleasedByUserId { get; set; }
    public Guid? ReleaseApplicationId { get; set; }

    public License License { get; set; } = null!;
    public Application? ReleaseApplication { get; set; }
}
