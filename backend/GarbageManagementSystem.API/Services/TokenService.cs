using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GarbageManagementSystem.API.Models.Entities;
using Microsoft.IdentityModel.Tokens;

namespace GarbageManagementSystem.API.Services;

public interface ITokenService
{
    /// <summary>Generates a signed JWT for the given user/role and returns the token plus its expiry.</summary>
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user, string role);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user, string role)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
        var expiryMinutes = int.TryParse(jwtSection["ExpiryInMinutes"], out var minutes) ? minutes : 1440;
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
