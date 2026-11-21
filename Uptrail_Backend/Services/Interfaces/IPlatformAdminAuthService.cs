using Uptrail_Backend.Models.Dtos.PlatformAdminDtos;

namespace Uptrail_Backend.Services.Interfaces
{
    public interface IPlatformAdminAuthService
    {
        Task<PlatformAdminTokenResponseDto> LoginAsync(PlatformAdminLoginDto request);
        Task<PlatformAdminTokenResponseDto> RefreshAsync(RefreshTokenRequestDto request);
        Task LogoutAsync(RefreshTokenRequestDto request);
    }
}
