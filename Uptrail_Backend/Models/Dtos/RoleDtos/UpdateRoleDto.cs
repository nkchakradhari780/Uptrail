using System.ComponentModel.DataAnnotations;

namespace Uptrail_Backend.Models.Dtos.RoleDtos
{
    public class UpdateRoleDto
    {
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? UpdatedBy { get; set; }
    }
}
