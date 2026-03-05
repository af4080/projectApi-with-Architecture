using projectApiAngular.Models;
using projectApiAngular.Validations;
using System.ComponentModel.DataAnnotations;

namespace projectApiAngular.DTO
{
    public class UserDto
    {
        public class CreateUserDto
        {
            [Required]
            [MaxLength(50)]
            public required string Name { get; set; }

            [Required]
            [StrongPassword]
            public required string Password { get; set; }

            [EmailAddress]
            [Required]
            [MaxLength(50)]
            public required string Email { get; set; }

            [Phone]
            [Required]
            [MaxLength(15)]
            public required string Phone { get; set; }

        }
        public class ReadUserDto
        {

            public int Id { get; set; }

            public required string Name { get; set; }

            [EmailAddress]
            public required string Email { get; set; }

            [Phone]
            public required string Phone { get; set; }

            public string? Role { get; set; } = "user";
        }
            public class LoginDto
            {
                [EmailAddress]
                [Required]
                [MaxLength(50)]
                public required string Email { get; set; }
                [StrongPassword]
                public required string Password { get; set; }
            }
        
    }
}
