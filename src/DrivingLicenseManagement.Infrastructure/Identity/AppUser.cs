using DrivingLicenseManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DrivingLicenseManagement.Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>
{
    public Guid PersonId { get; set; }
    public bool IsActive { get; set; }
    public Person Person { get; set; } = null!;

}