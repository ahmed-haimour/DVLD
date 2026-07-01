using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Auth.Dtos;
using DrivingLicenseManagement.Application.Features.Identity.Dtos;
using DrivingLicenseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DrivingLicenseManagement.Infrastructure.Identity;


public class TokenProvider(IConfiguration configuration, IAppDbContext context) : ITokenProvider
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IAppDbContext _context = context;

    public async Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default)
    {
        var tokenResult = await CreateAsync(user, ct);

        if (tokenResult.IsError)
        {
            return tokenResult.Error!;
        }

        return tokenResult.Value;
    }



    private async Task<Result<TokenResponse>> CreateAsync(AppUserDto user, CancellationToken ct)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;
        var key = jwtSettings["Secret"]!;

        var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenExpirationInMinutes"]!));

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()!),
            new (JwtRegisteredClaimNames.Email, user.Email!),
            new (ClaimTypes.Name, user.UserName!),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new(ClaimTypes.Role, role));
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var securityToken = tokenHandler.CreateToken(descriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        var now = DateTime.UtcNow;

        // Revoke any existing active refresh tokens for this user (token rotation).
        var activeToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.UserId == user.Id && !x.IsRevoked && x.ExpiresAt > now, ct);

        if (activeToken is not null)
        {
            activeToken.IsRevoked = true;
            activeToken.RevokedAt = now;
        }

        var refreshTokenValue = GenerateRefreshToken();
        var refreshTokenExpiresAt = now
            .AddDays(int.Parse(jwtSettings["RefreshTokenExpirationInDays"]!));

        var refreshToken = new RefreshToken
        {
            Token = refreshTokenValue,
            UserId = user.Id,
            CreatedAt = now,
            ExpiresAt = refreshTokenExpiresAt,
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(ct);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresOnUtc = expires,
        };
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}