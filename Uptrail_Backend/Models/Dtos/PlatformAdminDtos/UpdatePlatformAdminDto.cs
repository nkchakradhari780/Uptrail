namespace Uptrail_Backend.Models.Dtos.PlatformAdminDtos
{
    public class UpdatePlatformAdminDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public bool IsActive { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
