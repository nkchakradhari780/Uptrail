namespace Uptrail_Backend.Models.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PlatformAdminId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public bool Revoked { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public DateTime ExpiresAt { get; set; } 
    }
}
