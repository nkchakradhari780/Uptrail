namespace Uptrail_Backend.Models.Dtos.PlatformAdminDtos
{
    public class PlatformAdminTokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
