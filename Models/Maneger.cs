using System.ComponentModel.DataAnnotations;

namespace projectApiAngular.Models
{
    public class Maneger
    {
        public int Id { get; set; }
        public string Name { get; set; }
      //hashed password
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }


    }
}
