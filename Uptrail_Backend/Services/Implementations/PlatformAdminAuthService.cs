using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Uptrail_Backend.Common.Constants;
using Uptrail_Backend.Exceptions;
using Uptrail_Backend.Helpers.Interfaces;
using Uptrail_Backend.Models.Dtos.PlatformAdminDtos;
using Uptrail_Backend.Models.Entities;
using Uptrail_Backend.Repositories.Interfaces;
using Uptrail_Backend.Services.Interfaces;

namespace Uptrail_Backend.Services.Implementations
{
    public class PlatformAdminAuthService : IPlatformAdminAuthService
    {
        private readonly IPlatformAdminAuthRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;

        public PlatformAdminAuthService(
            IPlatformAdminAuthRepository repository,
            IPasswordHasher passwordHasher,
            IJwtHelper jwtHelper,
            IConfiguration configuration)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _jwtHelper = jwtHelper;
            _configuration = configuration;
        }

        public async Task<PlatformAdminTokenResponseDto> LoginAsync(PlatformAdminLoginDto request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new BadRequestException(ResponseMessages.EmailAndPasswordRequired);
            }

            var admin = await _repository.GetActiveByEmailAsync(request.Email);

            if (admin is null || !_passwordHasher.VerifyPassword(request.Password, admin.PasswordHash))
            {
                throw new UnauthorizedAccessException(ResponseMessages.InvalidEmailOrPassword);
            }

            await _repository.UpdateLastLoginAtAsync(admin.Id);
            return await IssueTokensAsync(ToResponse(admin));
        }

        public async Task<PlatformAdminTokenResponseDto> RefreshAsync(RefreshTokenRequestDto request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new BadRequestException(ResponseMessages.RefreshTokenRequired);
            }

            Guid adminId;
            try
            {
                adminId = _jwtHelper.ValidateRefreshToken(request.RefreshToken);
            }
            catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
            {
                throw new UnauthorizedAccessException(ResponseMessages.RefreshTokenInvalidOrExpired, ex);
            }

            var storedToken = await _repository.GetRefreshTokenByHashAsync(HashToken(request.RefreshToken));
            if (storedToken is null
                || storedToken.Revoked
                || storedToken.ExpiresAt <= DateTime.UtcNow
                || storedToken.PlatformAdminId != adminId)
            {
                throw new UnauthorizedAccessException(ResponseMessages.RefreshTokenInvalidOrExpired);
            }

            var admin = await _repository.GetActiveByIdAsync(adminId);
            if (admin is null)
            {
                throw new UnauthorizedAccessException(ResponseMessages.PlatformAdminInactiveOrNotFound);
            }

            await _repository.RevokeRefreshTokenAsync(storedToken);
            return await IssueTokensAsync(ToResponse(admin));
        }

        public async Task LogoutAsync(RefreshTokenRequestDto request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new BadRequestException(ResponseMessages.RefreshTokenRequired);
            }

            var storedToken = await _repository.GetRefreshTokenByHashAsync(HashToken(request.RefreshToken));
            if (storedToken is null || storedToken.Revoked)
            {
                return;
            }

            await _repository.RevokeRefreshTokenAsync(storedToken);
        }

        private async Task<PlatformAdminTokenResponseDto> IssueTokensAsync(PlatformAdminResponseDto admin)
        {
            var accessLifetime = TimeSpan.FromMinutes(GetPositiveSetting("AccessTokenExpirationMinutes", 60));
            var refreshLifetime = TimeSpan.FromDays(GetPositiveSetting("RefreshTokenExpirationDays", 7));
            var now = DateTime.UtcNow;
            var refreshToken = _jwtHelper.GenerateRefreshJwtToken(admin, refreshLifetime);

            await _repository.AddRefreshTokenAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                PlatformAdminId = admin.Id,
                UserId = null,
                TokenHash = HashToken(refreshToken),
                CreatedAt = now,
                ExpiresAt = now.Add(refreshLifetime)
            });

            return new PlatformAdminTokenResponseDto
            {
                AccessToken = _jwtHelper.GenerateAccessJwtToken(admin, accessLifetime),
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = now.Add(accessLifetime),
                RefreshTokenExpiresAt = now.Add(refreshLifetime)
            };
        }

        private int GetPositiveSetting(string key, int fallback)
        {
            var raw = _configuration[$"Jwt:{key}"]
                ?? Environment.GetEnvironmentVariable($"Jwt__{key}")
                ?? Environment.GetEnvironmentVariable($"JWT_{key.ToUpperInvariant()}");

            return int.TryParse(raw, out var value) && value > 0
                ? value
                : fallback;
        }

        private static string HashToken(string token)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }

        private static PlatformAdminResponseDto ToResponse(PlatformAdmin admin)
        {
            return new PlatformAdminResponseDto
            {
                Id = admin.Id,
                Name = admin.Name,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                ProfileImageUrl = admin.ProfileImageUrl,
                IsEmailVerified = admin.IsEmailVerified,
                IsActive = admin.IsActive,
                CreatedAt = admin.CreatedAt,
                UpdatedAt = admin.UpdatedAt,
                LastLoginAt = admin.LastLoginAt
            };
        }
    }
}
