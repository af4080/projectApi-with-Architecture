using System.ComponentModel.DataAnnotations;

namespace projectApiAngular.Models
{
    public enum Role
    {
        user,
        admin
    }
    public class User
    {
        public int Id { get; set; }

        public required string Password { get; set; }
        public required string Name { get; set; }

        [EmailAddress]
        public required string Email { get; set; }
        [Phone]
        public required string Phone { get; set; }

       public List<Purchase>? Purchases { get; set; }

        public List<Gift>? WonGifts { get; set; }

        public required Role Role { get; set; }




    }
}
