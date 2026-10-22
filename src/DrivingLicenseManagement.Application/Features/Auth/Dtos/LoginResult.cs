namespace DrivingLicenseManagement.Application.Features.Auth.Dtos;

public sealed record LoginResult(bool RequiresEmailVerification, TokenResponse? Tokens);