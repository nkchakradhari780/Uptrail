namespace Uptrail_Backend.Models.Entities
{
    public class Company: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Website { get; set; }
        public ICollection<User> Users { get; set; } = [];
        public ICollection<Role> Roles { get; set; } = [];
    }
}
