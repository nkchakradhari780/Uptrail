using Uptrail_Backend.Models.Dtos.PlatformAdminDtos;

namespace Uptrail_Backend.Helpers.Interfaces
{
    public interface IJwtHelper
    {
        string GenerateAccessJwtToken(PlatformAdminResponseDto admin, TimeSpan expiry);
        string GenerateRefreshJwtToken(PlatformAdminResponseDto admin, TimeSpan expiry);
        Guid ValidateRefreshToken(string refreshToken);
    }
}
