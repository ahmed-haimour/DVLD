using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Auth.Dtos;
using DrivingLicenseManagement.Application.Features.Identity.Dtos;

namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user,
    CancellationToken ct = default);
}