using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStoreWebAPI.Models;
using MyStoreWebAPI.Services;

namespace MyStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext context;

        public OrdersController(AppDbContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            int userId = JwtReader.GetUserId(User);
            string role = JwtReader.GetUserRole(User);

            Order? order = null;

            if (role == "admin")
            {
                order = context.Orders
                    .Include(o => o.User)
                    .Include(order => order.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefault(o => o.Id == id);
            }
            else
            {
                order = context.Orders
                    .Include(o => o.User)
                    .Include(order => order.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefault(o => o.Id == id && o.UserId == userId);
            }

            if (order == null)
            {
                return NotFound();
            }

            //get rid of object cycle
            foreach (var item in order.Items)
            {
                item.Order = null;
            }

            //hide password
            order.User.Password = "";

            return Ok(order);
        }
        
        
        [Authorize]
        [HttpGet]
        public IActionResult GetOrders(int? page)
        {
            int userId = JwtReader.GetUserId(User);
            string role = JwtReader.GetUserRole(User);

            IQueryable<Order> query = context.Orders.Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product);
            if(role != "admin")
            {
                query = query.Where(o => o.UserId == userId);
            }
            query = query.OrderByDescending(o=>o.Id);

            //pagination

            if(page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 5;
            int totalPages = 0;

            decimal count = query.Count();
            totalPages = (int)Math.Ceiling(count / pageSize);

            query = query.Skip((int)(page-1)*pageSize).Take(pageSize);


            // read orders
            var orders = query.ToList();

            
            foreach (var order in orders)
            {
                //get rid of object cycle
                foreach (var item in order.Items)
                {
                    item.Order = null!;
                }
                order.User.Password = "";
            }

            var response = new
            {
                Orders = orders,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page,
            };
            return Ok(response);
        }
        
        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder(OrderDto orderDto)
        {
            // check if payment method is valid

            if(!OrderHelper.PaymentMethods.ContainsKey(orderDto.PaymentMethod))
            {
                ModelState.AddModelError("Payment Method", "select a valid payment method");
                return BadRequest(ModelState);
            }

            int userId = JwtReader.GetUserId(User);
            var user = context.Users.Find(userId);
            if (user == null)
            {
                ModelState.AddModelError("Order", "unable to create order");
                return BadRequest(ModelState);
            }

            var productDictionary = OrderHelper.GetProductDictionary(orderDto.ProductIds);

            // create a new order

            Order order = new Order();
            order.UserId = userId;
            order.CreatedAt = DateTime.Now;
            order.ShippingFee = OrderHelper.ShippingFee;
            order.DeliveryAddress = orderDto.DeliveryAddress;
            order.PaymentMethod = orderDto.PaymentMethod;
            order.PaymentStatus = OrderHelper.PaymentStatus[0];//pending
            order.OrderStatus = OrderHelper.OrderStatus[0];//created

            foreach (var pair in productDictionary)
            {
                int productId = pair.Key;
                var product = context.Products.Find(productId);
                if (product == null)
                {
                    ModelState.AddModelError("Product", "Product with id "+ productId + "is not available");
                    return BadRequest(ModelState);
                }

                var orderItem = new OrderItem();
                orderItem.ProductId = productId;
                orderItem.Quantity = pair.Value;
                orderItem.UnitPrice = product.Price;

                order.Items.Add(orderItem);
            }

            if(order.Items.Count < 1)
            {
                ModelState.AddModelError("Order", "unable to create order");
                return BadRequest(ModelState);
            }

            // save order in database
            context.Orders.Add(order);
            context.SaveChanges();

            // get rid of object cycle

            foreach (var item in order.Items)
            {
                item.Order = null!;
            }

            //hide password
            order.User.Password = "";

            return Ok(order);
        }

        [Authorize(Roles ="admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, string? paymentStatus, string? orderStatus)
        {
            if(paymentStatus == null && orderStatus == null )
            {
                ModelState.AddModelError("Order", "unable to update order");
                return BadRequest(ModelState);
            }

            if(paymentStatus != null && !OrderHelper.PaymentStatus.Contains(paymentStatus))
            {
                ModelState.AddModelError("Payment", "payment status not valid");
                return BadRequest(ModelState);
            }

            if(orderStatus!= null && !OrderHelper.OrderStatus.Contains(orderStatus))
            {
                ModelState.AddModelError("Order", "order status not valid");
                return BadRequest(ModelState);
            }

            var order = context.Orders.Find(id);
            if(order == null)
            {
                return NotFound();
            }

            if (paymentStatus != null)
            {
                order.PaymentStatus = paymentStatus;
            } 
            
            if (orderStatus != null)
            {
                order.OrderStatus = orderStatus;
            }

            context.SaveChanges();
            return Ok(order);
        }


        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var order = context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            context.Orders.Remove(order);

            context.SaveChanges(); ;

            return Ok(order+"order is deleted");
        }

    }
}
