namespace Uptrail_Backend.Models.Dtos.PlatformAdminDtos
{
    public class CreatePlatformAdminDto
    {
        public required string Name { get; set; } 
        public required string Email { get; set; } 
        public required string Password { get; set; }
        public required string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
    }
}
