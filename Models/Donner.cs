using System.ComponentModel.DataAnnotations;

namespace projectApiAngular.Models
{
    public class Donner
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        [Phone]
        public required string Phone { get; set; }

        public  List<Gift>? Gifts { get; set; }

    }
}
