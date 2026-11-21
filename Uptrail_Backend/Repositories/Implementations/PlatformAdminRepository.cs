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
                Name = pAdmin.Name,
                Email = pAdmin.Email,
                PhoneNumber = pAdmin.PhoneNumber,
                ProfileImageUrl = pAdmin.ProfileImageUrl,
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
            var platformAdmin = await _context.PlatformAdmins
                .AsNoTracking()
                .Where(admin => admin.Email == email && admin.IsActive)
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
            var trimmedTerm = searchTerm.Trim();
            var lowerTerm = trimmedTerm.ToLower();

            var query = _context.PlatformAdmins
                .AsNoTracking()
                .Where(admin => admin.IsActive);

            if (Guid.TryParse(trimmedTerm, out var id))
            {
                query = query.Where(admin =>
                    admin.Id == id ||
                    admin.Name.ToLower().Contains(lowerTerm) ||
                    admin.Email.ToLower().Contains(lowerTerm) ||
                    admin.PhoneNumber.Contains(trimmedTerm));
            }
            else
            {
                query = query.Where(admin =>
                    admin.Name.ToLower().Contains(lowerTerm) ||
                    admin.Email.ToLower().Contains(lowerTerm) ||
                    admin.PhoneNumber.Contains(trimmedTerm));
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
                .FirstOrDefaultAsync(admin => admin.Id == platformAdminId);

            if (platformAdmin == null)
                return null;

            platformAdmin.Name = pAdmin.Name;
            platformAdmin.Email = pAdmin.Email;
            platformAdmin.PhoneNumber = pAdmin.PhoneNumber;
            platformAdmin.ProfileImageUrl = pAdmin.ProfileImageUrl;
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
                .FirstOrDefaultAsync(admin => admin.Id == platformAdminId);

            if (platformAdmin == null)
                return false;

            platformAdmin.IsActive = false;
            platformAdmin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PlatformAdminEmailExistsAsync(string email)
        {
            return await _context.PlatformAdmins.AnyAsync(pa => pa.Email == email && pa.IsActive);
        }

        public async Task<bool> PlatformAdminPhoneExistsAsync(string phoneNumber)
        {
            return await _context.PlatformAdmins.AnyAsync(pa => pa.PhoneNumber == phoneNumber && pa.IsActive);
        }
      
    }
}
