using DrivingLicenseManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Infrastructure.Identity;

public class IdentityService(UserManager<AppUser> userManager) : IIdentityService
{
    private readonly UserManager<AppUser> _userManager = userManager;

    public async Task<bool> PersonHasUserAccountAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _userManager.Users
            .AnyAsync(x => x.PersonId == personId,
                cancellationToken); // we use anyAsync here because its return true or false
    }

}