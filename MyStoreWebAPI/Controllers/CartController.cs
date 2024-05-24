using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyStoreWebAPI.Models;
using MyStoreWebAPI.Services;

namespace MyStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext context;

        public CartController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("PaymentMethods")]
        public IActionResult GetPaymentMethod()
        {
            return Ok(OrderHelper.PaymentMethods);
        }


        [HttpGet]
        public IActionResult GetCart(string Ids)
        {
            CartDto cartDto = new CartDto();
            cartDto.Items = new List<CartItemDto>();
            cartDto.Subtotal = 0;
            cartDto.ShippingFee = OrderHelper.ShippingFee;
            cartDto.TotalPrice = 0;

            var productDictionary = OrderHelper.GetProductDictionary(Ids);

            foreach (var pair in productDictionary)
            {
                int productId = pair.Key;
                var product = context.Products.Find(productId);
                if (product == null)
                {
                    continue;
                }
                var cartItemDto = new CartItemDto();
                cartItemDto.Product = product;
                cartItemDto.Quantity = pair.Value;

                cartDto.Items.Add(cartItemDto);
                cartDto.Subtotal += pair.Value * product.Price;
                cartDto.TotalPrice = cartDto.Subtotal + cartDto.ShippingFee;
            }

            return Ok(cartDto);
        }
    }
}
