namespace DrivingLicenseManagement.Application.Features.Auth.Dtos;
public class TokenResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresOnUtc { get; set; }
}