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
    public class PlatformAdminAuthController : ControllerBase
    {
        private readonly IPlatformAdminAuthService _authService;

        public PlatformAdminAuthController(IPlatformAdminAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<PlatformAdminTokenResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] PlatformAdminLoginDto request)
        {
            var tokens = await _authService.LoginAsync(request);
            return Ok(ApiResponseFactory.SuccessResponse(tokens, ResponseMessages.PlatformAdminLoggedIn));
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResponse<PlatformAdminTokenResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var tokens = await _authService.RefreshAsync(request);
            return Ok(ApiResponseFactory.SuccessResponse(tokens, ResponseMessages.TokenRefreshed));
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            await _authService.LogoutAsync(request);
            return Ok(ApiResponseFactory.SuccessResponse<object?>(null, ResponseMessages.PlatformAdminLoggedOut));
        }
    }
}
