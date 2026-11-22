using Microsoft.EntityFrameworkCore;
using Uptrail_Backend.Data;
using Uptrail_Backend.Models.Entities;
using Uptrail_Backend.Repositories.Interfaces;

namespace Uptrail_Backend.Repositories.Implementations
{
    public class PlatformAdminAuthRepository : IPlatformAdminAuthRepository
    {
        private readonly AppDbContext _context;

        public PlatformAdminAuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<PlatformAdmin?> GetActiveByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Task.FromResult<PlatformAdmin?>(null);
            }

            var normalized = email.Trim().ToLower();
            return _context.PlatformAdmins
                .AsNoTracking()
                .FirstOrDefaultAsync(admin => admin.Email.ToLower() == normalized && admin.IsActive);
        }

        public Task<PlatformAdmin?> GetActiveByIdAsync(Guid id)
        {
            return _context.PlatformAdmins
                .AsNoTracking()
                .FirstOrDefaultAsync(admin => admin.Id == id && admin.IsActive);
        }

        public Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash)
        {
            return _context.RefreshTokens
                .FirstOrDefaultAsync(token => token.TokenHash == tokenHash);
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
        {
            refreshToken.Revoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLastLoginAtAsync(Guid adminId)
        {
            var admin = await _context.PlatformAdmins.FirstOrDefaultAsync(pa => pa.Id == adminId);
            if (admin != null)
            {
                admin.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
