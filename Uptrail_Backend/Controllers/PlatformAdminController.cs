using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Uptrail_Backend.Common.Constants;
using Uptrail_Backend.Common.Responses;
using Uptrail_Backend.Models.Dtos.PlatformAdminDtos;
using Uptrail_Backend.Services.Interfaces;

namespace Uptrail_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformAdminController : ControllerBase
    {
        private readonly IPlatformAdminService _platformAdminService;

        public PlatformAdminController(IPlatformAdminService platformAdminService)
        {
            _platformAdminService = platformAdminService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreatePlatformAdminDto request)
        {
            var id = await _platformAdminService.CreatePlatformAdminAsync(request);
            var response = ApiResponseFactory.SuccessResponse(id, ResponseMessages.PlatformAdminCreated);

            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<PlatformAdminResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _platformAdminService.GetAllPlatformAdminsAsync();
            return Ok(ApiResponseFactory.SuccessResponse(admins, ResponseMessages.PlatformAdminsRetrieved));
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<PlatformAdminResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            var admins = await _platformAdminService.SearchPlatformAdminsAsync(searchTerm);
            return Ok(ApiResponseFactory.SuccessResponse(admins, ResponseMessages.PlatformAdminsRetrieved));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PlatformAdminResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var admin = await _platformAdminService.GetPlatformAdminByIdAsync(id);
            return Ok(ApiResponseFactory.SuccessResponse(admin, ResponseMessages.PlatformAdminRetrieved));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PlatformAdminResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlatformAdminDto request)
        {
            var admin = await _platformAdminService.UpdatePlatformAdminByIdAsync(id, request);
            return Ok(ApiResponseFactory.SuccessResponse(admin, ResponseMessages.PlatformAdminUpdated));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _platformAdminService.DeletePlatformAdminByIdAsync(id);
            return Ok(ApiResponseFactory.SuccessResponse<object?>(null, ResponseMessages.PlatformAdminDeleted));
        }
    }
}
