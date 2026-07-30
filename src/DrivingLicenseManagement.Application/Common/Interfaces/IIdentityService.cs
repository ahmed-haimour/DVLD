using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Identity.Dtos;
using DrivingLicenseManagement.Application.Features.Identity.Enums;

namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface IIdentityService
{
        Task<bool> PersonHasUserAccountAsync(Guid personId, CancellationToken ct = default);

        Task<Result<AppUserDto>> CreateUserAsync(Guid personId, string userName, string email, string password, string role, CancellationToken ct = default);

        Task<Result<AppUserDto>> AuthenticateAsync(string email, string password, CancellationToken ct = default);

        Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct = default);

        Task<Result<AppUserDto>> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
        Task<Result<PaginatedList<UserResponse>>> GetAllUsersAsync(
            UserFilterBy filterBy,
            string? filterValue,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default);

        Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken ct = default);
        Task<bool> AuthorizeAsync(Guid userId, string policyName, CancellationToken ct = default);
}