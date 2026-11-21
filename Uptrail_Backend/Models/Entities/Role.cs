namespace Uptrail_Backend.Models.Entities
{
    public class Role: BaseEntity
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public Company Company { get; set; } = null!;

    }
}
