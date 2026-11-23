using Microsoft.EntityFrameworkCore;
using Uptrail_Backend.Data;
using Uptrail_Backend.Models.Dtos.RoleDtos;
using Uptrail_Backend.Models.Entities;
using Uptrail_Backend.Repositories.Interfaces;

namespace Uptrail_Backend.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                CompanyId = createRoleDto.CompanyId,
                Name = createRoleDto.Name.Trim(),
                IsAdmin = createRoleDto.IsAdmin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createRoleDto.CreatedBy
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role.Id;
        }

        public async Task<List<RoleResponseDto>> GetAllRolesAsync(Guid? companyId = null)
        {
            var query = _context.Roles
                .AsNoTracking()
                .Where(r => r.IsActive);

            if (companyId.HasValue && companyId.Value != Guid.Empty)
            {
                query = query.Where(r => r.CompanyId == companyId.Value);
            }

            return await query
                .OrderBy(r => r.Name)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    CompanyId = r.CompanyId,
                    CompanyName = r.Company != null ? r.Company.Name : string.Empty,
                    Name = r.Name,
                    IsAdmin = r.IsAdmin,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    CreatedBy = r.CreatedBy,
                    UpdatedAt = r.UpdatedAt,
                    UpdatedBy = r.UpdatedBy
                })
                .ToListAsync();
        }

        public async Task<List<RoleResponseDto>> GetRolesByCompanyIdAsync(Guid companyId)
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(r => r.CompanyId == companyId && r.IsActive)
                .OrderBy(r => r.Name)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    CompanyId = r.CompanyId,
                    CompanyName = r.Company != null ? r.Company.Name : string.Empty,
                    Name = r.Name,
                    IsAdmin = r.IsAdmin,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    CreatedBy = r.CreatedBy,
                    UpdatedAt = r.UpdatedAt,
                    UpdatedBy = r.UpdatedBy
                })
                .ToListAsync();
        }

        public async Task<RoleResponseDto?> GetRoleByIdAsync(Guid roleId)
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(r => r.Id == roleId && r.IsActive)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    CompanyId = r.CompanyId,
                    CompanyName = r.Company != null ? r.Company.Name : string.Empty,
                    Name = r.Name,
                    IsAdmin = r.IsAdmin,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    CreatedBy = r.CreatedBy,
                    UpdatedAt = r.UpdatedAt,
                    UpdatedBy = r.UpdatedBy
                })
                .FirstOrDefaultAsync();
        }

        public async Task<RoleResponseDto?> UpdateRoleByIdAsync(Guid roleId, UpdateRoleDto updateRoleDto)
        {
            var role = await _context.Roles
                .Include(r => r.Company)
                .FirstOrDefaultAsync(r => r.Id == roleId && r.IsActive);

            if (role == null)
            {
                return null;
            }

            role.Name = updateRoleDto.Name.Trim();
            role.IsAdmin = updateRoleDto.IsAdmin;
            role.IsActive = updateRoleDto.IsActive;
            role.UpdatedAt = DateTime.UtcNow;
            role.UpdatedBy = updateRoleDto.UpdatedBy;

            await _context.SaveChangesAsync();

            return new RoleResponseDto
            {
                Id = role.Id,
                CompanyId = role.CompanyId,
                CompanyName = role.Company != null ? role.Company.Name : string.Empty,
                Name = role.Name,
                IsAdmin = role.IsAdmin,
                IsActive = role.IsActive,
                CreatedAt = role.CreatedAt,
                CreatedBy = role.CreatedBy,
                UpdatedAt = role.UpdatedAt,
                UpdatedBy = role.UpdatedBy
            };
        }

        public async Task<bool> DeleteRoleByIdAsync(Guid roleId)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == roleId && r.IsActive);

            if (role == null)
            {
                return false;
            }

            role.IsActive = false;
            role.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RoleResponseDto>> SearchRolesAsync(string searchTerm, Guid? companyId = null)
        {
            var trimmedTerm = searchTerm.Trim().Trim('"', '\'');
            var lowerTerm = trimmedTerm.ToLower();

            var query = _context.Roles
                .AsNoTracking()
                .Where(r => r.IsActive);

            if (companyId.HasValue && companyId.Value != Guid.Empty)
            {
                query = query.Where(r => r.CompanyId == companyId.Value);
            }

            if (Guid.TryParse(trimmedTerm, out var id))
            {
                query = query.Where(r =>
                    r.Id == id ||
                    r.CompanyId == id ||
                    r.Name.ToLower().Contains(lowerTerm) ||
                    (r.Company != null && r.Company.Name.ToLower().Contains(lowerTerm)));
            }
            else
            {
                query = query.Where(r =>
                    r.Name.ToLower().Contains(lowerTerm) ||
                    (r.Company != null && r.Company.Name.ToLower().Contains(lowerTerm)));
            }

            return await query
                .OrderBy(r => r.Name)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    CompanyId = r.CompanyId,
                    CompanyName = r.Company != null ? r.Company.Name : string.Empty,
                    Name = r.Name,
                    IsAdmin = r.IsAdmin,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    CreatedBy = r.CreatedBy,
                    UpdatedAt = r.UpdatedAt,
                    UpdatedBy = r.UpdatedBy
                })
                .ToListAsync();
        }

        public async Task<bool> RoleNameExistsInCompanyAsync(Guid companyId, string roleName, Guid? excludeRoleId = null)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return false;
            }

            var normalized = roleName.Trim().ToLower();
            var query = _context.Roles
                .Where(r => r.CompanyId == companyId && r.Name.ToLower() == normalized && r.IsActive);

            if (excludeRoleId.HasValue && excludeRoleId.Value != Guid.Empty)
            {
                query = query.Where(r => r.Id != excludeRoleId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> CompanyExistsAsync(Guid companyId)
        {
            return await _context.Companies.AnyAsync(c => c.Id == companyId && c.IsActive);
        }

        public async Task<bool> HasActiveUsersAsync(Guid roleId)
        {
            return await _context.Users.AnyAsync(u => u.RoleId == roleId && u.IsActive);
        }
    }
}
