using Microsoft.EntityFrameworkCore;
using Uptrail_Backend.Data;
using Uptrail_Backend.Models.Dtos.PlatformAdminDtos;
using Uptrail_Backend.Models.Entities;
using Uptrail_Backend.Repositories.Interfaces;

namespace Uptrail_Backend.Repositories.Implementations
{
    public class PlatformAdminRepository: IPlatformAdminRepository
    {
        private readonly AppDbContext _context;

        public PlatformAdminRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> CreatePlatformAdminAsync(CreatePlatformAdminDto pAdmin)
        {
            var newPlatformAdmin = new PlatformAdmin
            {
                Name = pAdmin.Name.Trim(),
                Email = pAdmin.Email.Trim().ToLowerInvariant(),
                PhoneNumber = pAdmin.PhoneNumber.Trim(),
                ProfileImageUrl = pAdmin.ProfileImageUrl?.Trim() ?? string.Empty,
                PasswordHash = pAdmin.Password
            };

            _context.PlatformAdmins.Add(newPlatformAdmin);
            await _context.SaveChangesAsync();
            return newPlatformAdmin.Id;
        }
        public async Task<List<PlatformAdminResponseDto>> GetAllPlatformAdminAsync()
        {
            var platformAdmins = await _context.PlatformAdmins
                .AsNoTracking()
                .Where(admin => admin.IsActive)
                .Select(admin => new PlatformAdminResponseDto
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
                })
                .ToListAsync();

            return platformAdmins;
        }
        public async Task<PlatformAdminResponseDto?> GetPlatformAdminByIdAsync(Guid platformAdminId)
        {
            var platformAdmin = await _context.PlatformAdmins
                .AsNoTracking()
                .Where(admin => admin.Id == platformAdminId && admin.IsActive)
                .Select(admin => new PlatformAdminResponseDto
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
                })
                .FirstOrDefaultAsync();

            return platformAdmin;
        }
        public async Task<PlatformAdminResponseDto?> GetPlatformAdminByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var normalized = email.Trim().ToLowerInvariant();
            var platformAdmin = await _context.PlatformAdmins
                .AsNoTracking()
                .Where(admin => admin.Email.ToLower() == normalized && admin.IsActive)
                .Select(admin => new PlatformAdminResponseDto
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
                })
                .FirstOrDefaultAsync();

            return platformAdmin;
        }

        public async Task<List<PlatformAdminResponseDto>> SearchPlatformAdminsAsync(string searchTerm)
        {
            var trimmedTerm = searchTerm.Trim().Trim('"', '\'');
            var lowerTerm = trimmedTerm.ToLower();
            var digitsOnly = new string(trimmedTerm.Where(char.IsDigit).ToArray());

            var query = _context.PlatformAdmins
                .AsNoTracking()
                .Where(admin => admin.IsActive);

            if (Guid.TryParse(trimmedTerm, out var id))
            {
                query = query.Where(admin =>
                    admin.Id == id ||
                    admin.Name.ToLower().Contains(lowerTerm) ||
                    admin.Email.ToLower().Contains(lowerTerm) ||
                    admin.PhoneNumber.Contains(trimmedTerm) ||
                    (!string.IsNullOrEmpty(digitsOnly) && admin.PhoneNumber.Contains(digitsOnly)));
            }
            else
            {
                query = query.Where(admin =>
                    admin.Name.ToLower().Contains(lowerTerm) ||
                    admin.Email.ToLower().Contains(lowerTerm) ||
                    admin.PhoneNumber.Contains(trimmedTerm) ||
                    (!string.IsNullOrEmpty(digitsOnly) && admin.PhoneNumber.Contains(digitsOnly)));
            }

            return await query
                .Select(admin => new PlatformAdminResponseDto
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
                })
                .ToListAsync();
        }
        public async Task<PlatformAdminResponseDto?> UpdatePlatformAdminByIdAsync(Guid platformAdminId, UpdatePlatformAdminDto pAdmin)
        {
            var platformAdmin = await _context.PlatformAdmins
                .FirstOrDefaultAsync(admin => admin.Id == platformAdminId && admin.IsActive);

            if (platformAdmin == null)
                return null;

            platformAdmin.Name = pAdmin.Name.Trim();
            platformAdmin.Email = pAdmin.Email.Trim().ToLowerInvariant();
            platformAdmin.PhoneNumber = pAdmin.PhoneNumber.Trim();
            platformAdmin.ProfileImageUrl = pAdmin.ProfileImageUrl?.Trim() ?? string.Empty;
            platformAdmin.IsEmailVerified = pAdmin.IsEmailVerified;
            platformAdmin.IsActive = pAdmin.IsActive;
            platformAdmin.UpdatedAt = DateTime.UtcNow;
            platformAdmin.UpdatedBy = pAdmin.UpdatedBy;

            await _context.SaveChangesAsync();

            return new PlatformAdminResponseDto
            {
                Id = platformAdmin.Id,
                Name = platformAdmin.Name,
                Email = platformAdmin.Email,
                PhoneNumber = platformAdmin.PhoneNumber,
                ProfileImageUrl = platformAdmin.ProfileImageUrl,
                IsEmailVerified = platformAdmin.IsEmailVerified,
                IsActive = platformAdmin.IsActive,
                CreatedAt = platformAdmin.CreatedAt,
                UpdatedAt = platformAdmin.UpdatedAt,
                LastLoginAt = platformAdmin.LastLoginAt
            };
        }
        public async Task<bool> DeletePlatformAdminById(Guid platformAdminId)
        {
            var platformAdmin = await _context.PlatformAdmins
                .FirstOrDefaultAsync(admin => admin.Id == platformAdminId && admin.IsActive);

            if (platformAdmin == null)
                return false;

            platformAdmin.IsActive = false;
            platformAdmin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PlatformAdminEmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var normalized = email.Trim().ToLowerInvariant();
            return await _context.PlatformAdmins.AnyAsync(pa => pa.Email.ToLower() == normalized && pa.IsActive);
        }

        public async Task<bool> PlatformAdminPhoneExistsAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
            var trimmed = phoneNumber.Trim();
            return await _context.PlatformAdmins.AnyAsync(pa => pa.PhoneNumber == trimmed && pa.IsActive);
        }
      
    }
}
