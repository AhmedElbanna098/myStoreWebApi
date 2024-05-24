using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyStoreWebAPI.Models
{
    public class ProductDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required,Precision(16, 2)]
        public decimal Price { get; set; }

        [MaxLength(4000)]
        public string? Description { get; set; }

        public IFormFile? ImageFile{ get; set; } 
    }
}
