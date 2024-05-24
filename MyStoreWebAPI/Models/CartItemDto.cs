namespace MyStoreWebAPI.Models
{
    public class CartItemDto
    {
        public Product Product { get; set; } = new();
        public int Quantity { get; set; }
    }
}
