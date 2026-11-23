using Microsoft.EntityFrameworkCore;
using Uptrail_Backend.Data;
using Uptrail_Backend.Models.Dtos.CompanyDtos;
using Uptrail_Backend.Models.Entities;
using Uptrail_Backend.Repositories.Interfaces;

namespace Uptrail_Backend.Repositories.Implementations
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _context;

        public CompanyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateCompanyAsync(CreateCompanyDto createCompanyDto)
        {
            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = createCompanyDto.Name.Trim(),
                Email = createCompanyDto.Email.Trim().ToLowerInvariant(),
                PhoneNumber = createCompanyDto.PhoneNumber.Trim(),
                Address = createCompanyDto.Address.Trim(),
                Website = string.IsNullOrWhiteSpace(createCompanyDto.Website) ? null : createCompanyDto.Website.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createCompanyDto.CreatedBy
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company.Id;
        }

        public async Task<List<CompanyResponseDto>> GetAllCompaniesAsync()
        {
            return await _context.Companies
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new CompanyResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    Website = c.Website,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedAt = c.UpdatedAt,
                    UpdatedBy = c.UpdatedBy,
                    ActiveUsersCount = c.Users.Count(u => u.IsActive),
                    ActiveRolesCount = c.Roles.Count(r => r.IsActive)
                })
                .ToListAsync();
        }

        public async Task<CompanyResponseDto?> GetCompanyByIdAsync(Guid companyId)
        {
            return await _context.Companies
                .AsNoTracking()
                .Where(c => c.Id == companyId && c.IsActive)
                .Select(c => new CompanyResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    Website = c.Website,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedAt = c.UpdatedAt,
                    UpdatedBy = c.UpdatedBy,
                    ActiveUsersCount = c.Users.Count(u => u.IsActive),
                    ActiveRolesCount = c.Roles.Count(r => r.IsActive)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CompanyResponseDto?> UpdateCompanyByIdAsync(Guid companyId, UpdateCompanyDto updateCompanyDto)
        {
            var company = await _context.Companies
                .Include(c => c.Users)
                .Include(c => c.Roles)
                .FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive);

            if (company == null)
            {
                return null;
            }

            company.Name = updateCompanyDto.Name.Trim();
            company.Email = updateCompanyDto.Email.Trim().ToLowerInvariant();
            company.PhoneNumber = updateCompanyDto.PhoneNumber.Trim();
            company.Address = updateCompanyDto.Address.Trim();
            company.Website = string.IsNullOrWhiteSpace(updateCompanyDto.Website) ? null : updateCompanyDto.Website.Trim();
            company.IsActive = updateCompanyDto.IsActive;
            company.UpdatedAt = DateTime.UtcNow;
            company.UpdatedBy = updateCompanyDto.UpdatedBy;

            await _context.SaveChangesAsync();

            return new CompanyResponseDto
            {
                Id = company.Id,
                Name = company.Name,
                Email = company.Email,
                PhoneNumber = company.PhoneNumber,
                Address = company.Address,
                Website = company.Website,
                IsActive = company.IsActive,
                CreatedAt = company.CreatedAt,
                CreatedBy = company.CreatedBy,
                UpdatedAt = company.UpdatedAt,
                UpdatedBy = company.UpdatedBy,
                ActiveUsersCount = company.Users.Count(u => u.IsActive),
                ActiveRolesCount = company.Roles.Count(r => r.IsActive)
            };
        }

        public async Task<bool> DeleteCompanyByIdAsync(Guid companyId)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive);

            if (company == null)
            {
                return false;
            }

            company.IsActive = false;
            company.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CompanyResponseDto>> SearchCompaniesAsync(string searchTerm)
        {
            var trimmedTerm = searchTerm.Trim().Trim('"', '\'');
            var lowerTerm = trimmedTerm.ToLower();

            var query = _context.Companies
                .AsNoTracking()
                .Where(c => c.IsActive);

            if (Guid.TryParse(trimmedTerm, out var id))
            {
                query = query.Where(c =>
                    c.Id == id ||
                    c.Name.ToLower().Contains(lowerTerm) ||
                    c.Email.ToLower().Contains(lowerTerm) ||
                    c.PhoneNumber.Contains(trimmedTerm) ||
                    c.Address.ToLower().Contains(lowerTerm) ||
                    (c.Website != null && c.Website.ToLower().Contains(lowerTerm)));
            }
            else
            {
                query = query.Where(c =>
                    c.Name.ToLower().Contains(lowerTerm) ||
                    c.Email.ToLower().Contains(lowerTerm) ||
                    c.PhoneNumber.Contains(trimmedTerm) ||
                    c.Address.ToLower().Contains(lowerTerm) ||
                    (c.Website != null && c.Website.ToLower().Contains(lowerTerm)));
            }

            return await query
                .OrderBy(c => c.Name)
                .Select(c => new CompanyResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    Website = c.Website,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedAt = c.UpdatedAt,
                    UpdatedBy = c.UpdatedBy,
                    ActiveUsersCount = c.Users.Count(u => u.IsActive),
                    ActiveRolesCount = c.Roles.Count(r => r.IsActive)
                })
                .ToListAsync();
        }

        public async Task<bool> CompanyEmailExistsAsync(string email, Guid? excludeCompanyId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var normalized = email.Trim().ToLowerInvariant();
            var query = _context.Companies
                .Where(c => c.Email.ToLower() == normalized && c.IsActive);

            if (excludeCompanyId.HasValue && excludeCompanyId.Value != Guid.Empty)
            {
                query = query.Where(c => c.Id != excludeCompanyId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasActiveUsersOrRolesAsync(Guid companyId)
        {
            var hasUsers = await _context.Users.AnyAsync(u => u.CompanyId == companyId && u.IsActive);
            if (hasUsers) return true;

            return await _context.Roles.AnyAsync(r => r.CompanyId == companyId && r.IsActive);
        }
    }
}
