using System.ComponentModel.DataAnnotations;

namespace Uptrail_Backend.Models.Dtos.CompanyDtos
{
    public class CreateCompanyDto
    {
        [Required]
        [MaxLength(150)]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public required string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public required string PhoneNumber { get; set; }

        [Required]
        [MaxLength(500)]
        public required string Address { get; set; }

        [MaxLength(2048)]
        public string? Website { get; set; }

        public Guid? CreatedBy { get; set; }
    }
}
