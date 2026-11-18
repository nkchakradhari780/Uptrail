namespace Uptrail_Backend.Models.Entities
{
    public class Company
    {
        public Guid Id { get; set; }
        public Guid PlatformAdminId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Website { get; set; }
        public bool IsActive { get; set; } = true;
        public PlatformAdmin PlatformAdmin { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<User> Users { get; set; } = [];
        public ICollection<Role> Roles { get; set; } = [];
    }
}
