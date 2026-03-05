using projectApiAngular.Models;
using System.ComponentModel.DataAnnotations;

namespace projectApiAngular.DTO
{
    public class GiftDto
    {
        public class ReadGiftDto
        {
            public  int Id { get; set; }
            public required string Name { get; set; }
            public required string Description { get; set; }
            public  int Price { get; set; } = 10;
            public required string ImagePath { get; set; }
            public int CategoryId { get; set; }
            public required string CategoryName { get; set; }
            public int DonerId { get; set; }

            public required string DonerName { get; set; }



        }
        public class CreateGiftDto
        {
            [Required]
            [MaxLength(50)]
            public  required string Name { get; set; }
            [Required]
            [MaxLength(250)]
            public required string Description { get; set; }
            [Required]
            public int Price { get; set; }
            public int DonerId { get; set; }
            [Required]
            [MaxLength(100)]
            public required string ImagePath { get; set; }
            [Required]
            public int CategoryId { get; set; }
         
        }

        public class UpdateGiftDto
        {
            [MaxLength(50)]
            public string? Name { get; set; }

            [MaxLength(250)]
            public string? Description { get; set; }

            [Range(10, 1000)]
            public int? Price { get; set; }

            [MaxLength(100)]
            public string? ImagePath { get; set; }

            public int? CategoryId { get; set; }

        }
    }
}
