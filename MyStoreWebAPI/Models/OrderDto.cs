using System.ComponentModel.DataAnnotations;

namespace MyStoreWebAPI.Models
{
    public class OrderDto
    {
        [Required]
        public string ProductIds { get; set; } = "";

        [Required, MinLength(10), MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";

        [Required]
        public string PaymentMethod { get; set; } = "";
    }
}
