using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Uptrail_Backend.Common.Constants;
using Uptrail_Backend.Common.Responses;
using Uptrail_Backend.Models.Dtos.RoleDtos;
using Uptrail_Backend.Services.Interfaces;

namespace Uptrail_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        private Guid? GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;
            return Guid.TryParse(userIdStr, out var id) ? id : null;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateRoleDto request)
        {
            if (request != null && (!request.CreatedBy.HasValue || request.CreatedBy.Value == Guid.Empty))
            {
                request.CreatedBy = GetCurrentUserId();
            }

            var id = await _roleService.CreateRoleAsync(request!);
            var response = ApiResponseFactory.SuccessResponse(id, ResponseMessages.RoleCreated);
            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<RoleResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromQuery] Guid? companyId)
        {
            var roles = await _roleService.GetAllRolesAsync(companyId);
            return Ok(ApiResponseFactory.SuccessResponse(roles, ResponseMessages.RolesRetrieved));
        }

        [HttpGet("company/{companyId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<List<RoleResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCompanyId(Guid companyId)
        {
            var roles = await _roleService.GetRolesByCompanyIdAsync(companyId);
            return Ok(ApiResponseFactory.SuccessResponse(roles, ResponseMessages.RolesRetrieved));
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<RoleResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] Guid? companyId)
        {
            var roles = await _roleService.SearchRolesAsync(searchTerm, companyId);
            return Ok(ApiResponseFactory.SuccessResponse(roles, ResponseMessages.RolesRetrieved));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            return Ok(ApiResponseFactory.SuccessResponse(role, ResponseMessages.RoleRetrieved));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleDto request)
        {
            if (request != null && (!request.UpdatedBy.HasValue || request.UpdatedBy.Value == Guid.Empty))
            {
                request.UpdatedBy = GetCurrentUserId();
            }

            var role = await _roleService.UpdateRoleByIdAsync(id, request!);
            return Ok(ApiResponseFactory.SuccessResponse(role, ResponseMessages.RoleUpdated));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _roleService.DeleteRoleByIdAsync(id);
            return Ok(ApiResponseFactory.SuccessResponse<object?>(null, ResponseMessages.RoleDeleted));
        }
    }
}
