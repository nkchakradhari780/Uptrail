using Uptrail_Backend.Models.Dtos.RoleDtos;

namespace Uptrail_Backend.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Guid> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<List<RoleResponseDto>> GetAllRolesAsync(Guid? companyId = null);
        Task<List<RoleResponseDto>> GetRolesByCompanyIdAsync(Guid companyId);
        Task<RoleResponseDto> GetRoleByIdAsync(Guid roleId);
        Task<RoleResponseDto> UpdateRoleByIdAsync(Guid roleId, UpdateRoleDto updateRoleDto);
        Task DeleteRoleByIdAsync(Guid roleId);
        Task<List<RoleResponseDto>> SearchRolesAsync(string searchTerm, Guid? companyId = null);
    }
}
