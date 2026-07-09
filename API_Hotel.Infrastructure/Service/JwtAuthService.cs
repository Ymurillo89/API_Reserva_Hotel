using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API_Hotel.Infrastructure.Infrastructure.Services; 

public interface IJwtAuthService
{
    string GenerateToken(string username, string role);
}

public class JwtAuthService : IJwtAuthService
{
    private readonly IConfiguration _configuration;

    public JwtAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string username, string role)
    {
        var secret = _configuration["Jwt:SecretKey"] ?? "SUPER_SECRET_KEY_FOR_JWT_API_HOTEL_2026_LEAD_DEV_LONG_KEY!!";
        var issuer = _configuration["Jwt:Issuer"] ?? "API_Hotel";
        var audience = _configuration["Jwt:Audience"] ?? "API_Hotel_Clients";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}