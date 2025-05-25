namespace WebApplication3.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Product Product { get; set; }
    }
} 