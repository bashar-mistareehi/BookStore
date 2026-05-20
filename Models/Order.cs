namespace BookStore.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; } 
        public decimal? PriceTotal { get; set; }
        public DateTime? OrderDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
