using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class EmailVerificationCode : Entity
{
    public Guid UserId { get; set; }

    public string CodeHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime SentAt { get; set; }

    public int FailedAttempts { get; set; }
}