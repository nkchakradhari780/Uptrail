using System.ComponentModel.DataAnnotations;

namespace Uptrail_Backend.Models.Dtos.RoleDtos
{
    public class CreateRoleDto
    {
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        public bool IsAdmin { get; set; } = false;

        public Guid? CreatedBy { get; set; }
    }
}
