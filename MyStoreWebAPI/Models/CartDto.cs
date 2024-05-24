namespace MyStoreWebAPI.Models
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
