namespace projectApiAngular.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        //customerId
        public int CustomerId { get; set; }
        public  User Castomer { get; set; }
           
        public int GiftId { get; set; }

        public  Gift Gift { get; set; }
        
        public DateTime PurchaseDate { get; set; }



    }
}
