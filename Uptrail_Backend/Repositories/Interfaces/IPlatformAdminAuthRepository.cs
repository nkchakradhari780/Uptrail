using Uptrail_Backend.Models.Entities;

namespace Uptrail_Backend.Repositories.Interfaces
{
    public interface IPlatformAdminAuthRepository
    {
        Task<PlatformAdmin?> GetActiveByEmailAsync(string email);
        Task<PlatformAdmin?> GetActiveByIdAsync(Guid id);
        Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task RevokeRefreshTokenAsync(RefreshToken refreshToken);
    }
}
