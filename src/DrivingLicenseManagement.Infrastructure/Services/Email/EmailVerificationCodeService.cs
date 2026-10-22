using System.Security.Cryptography;
using System.Text;
using DrivingLicenseManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DrivingLicenseManagement.Infrastructure.Services.Email;

public sealed class EmailVerificationCodeService : IEmailVerificationCodeService
{

    private readonly byte[] _secret;

    public EmailVerificationCodeService(IConfiguration configuration)
    {
        var secret = configuration["EmailVerification:Secret"]
            ?? throw new InvalidOperationException(
                "Email verification secret is not configured.");

        _secret = Convert.FromBase64String(secret);
    }

    public string GenerateCode()
    {
        var number = RandomNumberGenerator.GetInt32(0, 1_000_000);

        return number.ToString("D6");
    }

    public string HashCode(string code)
    {
        using var hmac = new HMACSHA256(_secret);

        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(code));

        return Convert.ToBase64String(hash);
    }

    public bool VerifyCode(string code, string codeHash)
    {
        var expectedHash = Convert.FromBase64String(codeHash);

        using var hmac = new HMACSHA256(_secret);

        var actualHash = hmac.ComputeHash(
            Encoding.UTF8.GetBytes(code));

        return CryptographicOperations.FixedTimeEquals(
            actualHash,
            expectedHash);
    }
}