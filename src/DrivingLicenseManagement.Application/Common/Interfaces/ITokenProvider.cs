using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Identity;

namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(string userId, string role);
}