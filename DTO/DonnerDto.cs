using System.ComponentModel.DataAnnotations;

namespace projectApiAngular.DTO
{
    public class DonnerDto
    {
        public class CreateDonnerDto
        {
            [Required]
            [MaxLength(50)]
            public required string Name { get; set; }

            [Required]
            [EmailAddress]
            [MaxLength(50)]
            public required string Email { get; set; }

            [Required]
            [MaxLength(15)]
            [Phone]
            public required string Phone { get; set; } 
        }

        public class UpdateDonnerDto
        {
            [MaxLength(50)]
            public string? Name { get; set; }

            [EmailAddress]
            [MaxLength(50)]
            public string? Email { get; set; }

            [MaxLength(15)]
            [Phone]
            public string? Phone { get; set; }
        }

        public class ReadDonnerDto
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public required string Email { get; set; }
            public required string Phone { get; set; }
        }
    }
}
