namespace projectApiAngular.Models
{
    public class Basket
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public int GiftId { get; set; }
        public Gift? Gift{ get; set; }

        public int Amount { get; set; } =1;

    }
}
