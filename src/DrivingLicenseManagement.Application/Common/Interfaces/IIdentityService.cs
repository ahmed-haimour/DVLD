using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Identity.Dtos;

namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface IIdentityService
{
        Task<bool> PersonHasUserAccountAsync(Guid personId, CancellationToken ct = default);

        Task<Result<AppUserDto>> AuthenticateAsync(string email, string password, CancellationToken ct = default);
        Task<Result<AppUserDto>> GetUserByIdAsync(Guid userId, CancellationToken ct = default);

        Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken ct = default);
        Task<bool> AuthorizeAsync(Guid userId, string policyName, CancellationToken ct = default);
}