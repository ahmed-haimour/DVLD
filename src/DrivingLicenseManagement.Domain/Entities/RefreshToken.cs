using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class RefreshToken : Entity
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }

    // Computed property - not stored in DB
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
    
}
