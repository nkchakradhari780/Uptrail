using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Uptrail_Backend.Helpers.Interfaces;
using Uptrail_Backend.Models.Dtos.PlatformAdminDtos;

namespace Uptrail_Backend.Helpers.Implementations
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GenerateAccessJwtToken(PlatformAdminResponseDto admin, TimeSpan expiry)
        {
            return GenerateToken(admin, expiry, "access");
        }

        public string GenerateRefreshJwtToken(PlatformAdminResponseDto admin, TimeSpan expiry)
        {
            return GenerateToken(admin, expiry, "refresh");
        }

        public Guid ValidateRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new SecurityTokenException("The refresh token is required.");
            }

            var principal = new JwtSecurityTokenHandler().ValidateToken(
                refreshToken,
                CreateValidationParameters(),
                out _);

            var tokenType = principal.FindFirst("token_type")?.Value;
            if (!string.Equals(tokenType, "refresh", StringComparison.Ordinal))
            {
                throw new SecurityTokenException("The token is not a refresh token.");
            }

            var subject = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(subject, out var adminId))
            {
                throw new SecurityTokenException("The refresh token does not contain a valid administrator ID.");
            }

            return adminId;
        }

        private string GenerateToken(
            PlatformAdminResponseDto admin,
            TimeSpan expiry,
            string tokenType)
        {
            ArgumentNullException.ThrowIfNull(admin);

            if (expiry <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(expiry), "Token expiry must be greater than zero.");
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, admin.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Name, admin.Name),
                new("token_type", tokenType),
                new("is_active", admin.IsActive.ToString().ToLowerInvariant()),
                new("email_verified", admin.IsEmailVerified.ToString().ToLowerInvariant())
            };

            var credentials = new SigningCredentials(
                GetSigningKey(),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: GetRequiredSetting("Issuer"),
                audience: GetRequiredSetting("Audience"),
                claims: claims,
                expires: DateTime.UtcNow.Add(expiry),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private TokenValidationParameters CreateValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSigningKey(),
                ValidateIssuer = true,
                ValidIssuer = GetRequiredSetting("Issuer"),
                ValidateAudience = true,
                ValidAudience = GetRequiredSetting("Audience"),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

        private SymmetricSecurityKey GetSigningKey()
        {
            var key = GetRequiredSetting("Key");
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        private string GetRequiredSetting(string name)
        {
            var value = _configuration[$"Jwt:{name}"]
                ?? Environment.GetEnvironmentVariable($"Jwt__{name}")
                ?? Environment.GetEnvironmentVariable($"JWT_{name.ToUpperInvariant()}");

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"JWT configuration value 'Jwt:{name}' is missing.");
            }

            return value;
        }
    }
}
