using System.ComponentModel.DataAnnotations.Schema;

namespace projectApiAngular.Models
{
    public class Gift
    {

        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Price { get; set; } = 10;
        public int DonerId { get; set; }
        public  Donner Doner { get; set; }
        public required string  ImagePath { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int? WinnerId { get; set; }
        public User? Winner { get; set; }

        public List<Purchase> Purchases { get; set; } = new();







    }
}
