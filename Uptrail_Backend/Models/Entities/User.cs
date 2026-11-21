namespace Uptrail_Backend.Models.Entities
{
    public class User: BaseEntity
    {
        public Guid RoleId { get; set; }
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; } = false;
        public Role Role { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
