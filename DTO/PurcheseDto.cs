using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectApiAngular.DTO
{
    public class PurcheseDto
    {
        public class CreatePurcheseDto
        {
            [Required]
            [ForeignKey("Customer")]
            public int CustomerId { get; set; }

            [Required]
            [ForeignKey("Gift")]
            public int GiftId { get; set; }

            public DateTime PurchDate { get; set; } = DateTime.Now;
        }
        public class ReadPurcheseDto
        {
            public int Id { get; set; }

            public int CustomerId { get; set; }
            public string? CustomerName { get; set; }
            public string? CustomerEmail { get; set; }

            public int GiftId { get; set; }
            public string? GiftName { get; set; }
            public int? GiftPrice { get; set; }

            public DateTime PurchDate { get; set; }
        }
    }
}
