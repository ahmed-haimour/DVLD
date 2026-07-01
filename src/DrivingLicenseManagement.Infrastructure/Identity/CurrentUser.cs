using System.Security.Claims;
using DrivingLicenseManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DrivingLicenseManagement.Infrastructure.Identity;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            // The JWT carries the id in "sub"; ASP.NET may remap it to NameIdentifier. Check both.
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user?.FindFirstValue("sub");

            return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
        }
    }
}
