using Uptrail_Backend.Common.Constants;
using Uptrail_Backend.Exceptions;
using Uptrail_Backend.Models.Dtos.RoleDtos;
using Uptrail_Backend.Repositories.Interfaces;
using Uptrail_Backend.Services.Interfaces;

namespace Uptrail_Backend.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Guid> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            if (createRoleDto == null)
            {
                throw new BadRequestException("Role data is required.");
            }

            if (createRoleDto.CompanyId == Guid.Empty)
            {
                throw new BadRequestException(ResponseMessages.CompanyIdRequired);
            }

            if (string.IsNullOrWhiteSpace(createRoleDto.Name))
            {
                throw new BadRequestException(ResponseMessages.RoleNameRequired);
            }

            if (!await _roleRepository.CompanyExistsAsync(createRoleDto.CompanyId))
            {
                throw new NotFoundException(string.Format(ResponseMessages.CompanyNotFoundWithId, createRoleDto.CompanyId));
            }

            if (await _roleRepository.RoleNameExistsInCompanyAsync(createRoleDto.CompanyId, createRoleDto.Name))
            {
                throw new BadRequestException(ResponseMessages.RoleAlreadyExists);
            }

            return await _roleRepository.CreateRoleAsync(createRoleDto);
        }

        public async Task<List<RoleResponseDto>> GetAllRolesAsync(Guid? companyId = null)
        {
            if (companyId.HasValue && companyId.Value == Guid.Empty)
            {
                throw new BadRequestException(ResponseMessages.CompanyIdRequired);
            }

            var roles = await _roleRepository.GetAllRolesAsync(companyId);
            if (roles is null || roles.Count == 0)
            {
                throw new NotFoundException(ResponseMessages.NoRolesExist);
            }

            return roles;
        }

        public async Task<List<RoleResponseDto>> GetRolesByCompanyIdAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new BadRequestException(ResponseMessages.CompanyIdRequired);
            }

            if (!await _roleRepository.CompanyExistsAsync(companyId))
            {
                throw new NotFoundException(string.Format(ResponseMessages.CompanyNotFoundWithId, companyId));
            }

            var roles = await _roleRepository.GetRolesByCompanyIdAsync(companyId);
            if (roles is null || roles.Count == 0)
            {
                throw new NotFoundException(ResponseMessages.NoRolesExist);
            }

            return roles;
        }

        public async Task<RoleResponseDto> GetRoleByIdAsync(Guid roleId)
        {
            if (roleId == Guid.Empty)
            {
                throw new BadRequestException(ResponseMessages.RoleIdRequired);
            }

            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            return role ?? throw new NotFoundException(string.Format(ResponseMessages.RoleNotFoundWithId, roleId));
        }

        public async Task<RoleResponseDto> UpdateRoleByIdAsync(Guid roleId, UpdateRoleDto updateRoleDto)
        {
            if (roleId == Guid.Empty)
            {
                throw new BadRequestException(ResponseMessages.RoleIdRequired);
            }

            if (updateRoleDto == null || string.IsNullOrWhiteSpace(updateRoleDto.Name))
            {
                throw new BadRequestException(ResponseMessages.RoleNameRequired);
            }

            var existingRole = await _roleRepository.GetRoleByIdAsync(roleId);
            if (existingRole == null)
            {
                throw new NotFoundException(string.Format(ResponseMessages.RoleNotFoundWithId, roleId));
            }

            if (await _roleRepository.RoleNameExistsInCompanyAsync(existingRole.CompanyId, updateRoleDto.Name, roleId))
            {
                throw new BadRequestException(ResponseMessages.RoleAlreadyExists);
            }

            var updatedRole = await _roleRepository.UpdateRoleByIdAsync(roleId, updateRoleDto);
            return updatedRole ?? throw new NotFoundException(string.Format(ResponseMessages.RoleNotFoundWithId, roleId));
        }

        public async Task DeleteRoleByIdAsync(Guid roleId)
        {
            if (roleId == Guid.Empty)
            {
                throw new BadRequestException(ResponseMessages.RoleIdRequired);
            }

            var existingRole = await _roleRepository.GetRoleByIdAsync(roleId);
            if (existingRole == null)
            {
                throw new NotFoundException(string.Format(ResponseMessages.RoleNotFoundWithId, roleId));
            }

            if (await _roleRepository.HasActiveUsersAsync(roleId))
            {
                throw new BadRequestException(ResponseMessages.RoleHasActiveUsers);
            }

            if (!await _roleRepository.DeleteRoleByIdAsync(roleId))
            {
                throw new NotFoundException(string.Format(ResponseMessages.RoleNotFoundWithId, roleId));
            }
        }

        public async Task<List<RoleResponseDto>> SearchRolesAsync(string searchTerm, Guid? companyId = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new BadRequestException(ResponseMessages.RoleSearchTermRequired);
            }

            var cleanTerm = searchTerm.Trim().Trim('"', '\'');
            if (cleanTerm.StartsWith("?searchTerm=", StringComparison.OrdinalIgnoreCase))
            {
                cleanTerm = cleanTerm.Substring("?searchTerm=".Length).Trim().Trim('"', '\'');
            }
            else if (cleanTerm.StartsWith("searchTerm=", StringComparison.OrdinalIgnoreCase))
            {
                cleanTerm = cleanTerm.Substring("searchTerm=".Length).Trim().Trim('"', '\'');
            }

            if (string.IsNullOrWhiteSpace(cleanTerm))
            {
                throw new BadRequestException(ResponseMessages.RoleSearchTermRequired);
            }

            var roles = await _roleRepository.SearchRolesAsync(cleanTerm, companyId);
            if (roles is null || roles.Count == 0)
            {
                throw new NotFoundException(ResponseMessages.NoRolesExist);
            }

            return roles;
        }
    }
}
