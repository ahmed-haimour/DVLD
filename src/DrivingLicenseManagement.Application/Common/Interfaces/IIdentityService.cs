namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface IIdentityService
{
        Task<bool> PersonHasUserAccountAsync(
        Guid personId,
        CancellationToken cancellationToken = default);
}