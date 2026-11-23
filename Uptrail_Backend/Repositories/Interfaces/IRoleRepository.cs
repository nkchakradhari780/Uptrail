using Uptrail_Backend.Models.Dtos.RoleDtos;

namespace Uptrail_Backend.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Guid> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<List<RoleResponseDto>> GetAllRolesAsync(Guid? companyId = null);
        Task<List<RoleResponseDto>> GetRolesByCompanyIdAsync(Guid companyId);
        Task<RoleResponseDto?> GetRoleByIdAsync(Guid roleId);
        Task<RoleResponseDto?> UpdateRoleByIdAsync(Guid roleId, UpdateRoleDto updateRoleDto);
        Task<bool> DeleteRoleByIdAsync(Guid roleId);
        Task<List<RoleResponseDto>> SearchRolesAsync(string searchTerm, Guid? companyId = null);
        Task<bool> RoleNameExistsInCompanyAsync(Guid companyId, string roleName, Guid? excludeRoleId = null);
        Task<bool> CompanyExistsAsync(Guid companyId);
        Task<bool> HasActiveUsersAsync(Guid roleId);
    }
}
