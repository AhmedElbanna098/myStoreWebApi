using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyStoreWebAPI.Models;
using MyStoreWebAPI.Services;
using System.Linq.Expressions;

namespace MyStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment env;

        private readonly List<string> listCategories = new()
        {
            "Electronics", "Accessories", "Footwear", "Home Appliances", "Personal Care"
        };

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }

        [HttpGet("Categories")]
        public IActionResult GetCategories()
        {
            
            return Ok(listCategories);
        }

        [HttpGet]
        public IActionResult GetProducts(
            int? page,
            string? search, string? category,
            int? minPrice, int? maxPrice,
            string? sort, string? order
            )
        {
            IQueryable<Product> query = context.Products;

            //search functionality
            if(search != null)
            {
                query = query.Where(p=>
                p.Name.Contains(search) ||
                p.Description.Contains(search) ||
                p.Brand.Contains(search) ||
                p.Category.Contains(search));
            }

            if(category != null)
            {
                query = query.Where(p=>p.Category == category);
            }

            if(minPrice != null)
            {
                query = query.Where(p=>p.Price >= minPrice);
            }

            if(maxPrice != null)
            {
                query = query.Where(p=>p.Price <= maxPrice);
            }

            //sort functionality

            #region sorting functionality 1
            /*if (sort == null) sort = "id";
            if (order == null || order != "asc") order = "desc";

            //sort by name,brand,category,price,date
            if (sort.ToLower() == "name")
            {
                if(order == "asc")
                {
                    query = query.OrderBy(p=>p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p=>p.Name);
                }
            }

            else if (sort.ToLower() == "brand")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
            else if (sort.ToLower() == "category")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }
            else if (sort.ToLower() == "price")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }
            else if (sort.ToLower() == "date")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            } else
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }*/
            #endregion

            #region working sorting functionality
            if (sort == null) sort = "id";
            if (order == null || order != "asc") order = "desc";
            var sortMapping = new Dictionary<string, Expression<Func<Product, object>>>
            {
                { "name", p => p.Name },
                { "brand", p => p.Brand },
                { "category", p => p.Category },
                { "price", p => p.Price },
                { "date", p => p.CreatedAt },
                { "id", p => p.Id }
            };
            if (!sortMapping.ContainsKey(sort.ToLower())) sort = "id";

            if (order.ToLower() == "asc")
            {
                query = query.OrderBy(sortMapping[sort.ToLower()]);
            }
            else
            {
                query = query.OrderByDescending(sortMapping[sort.ToLower()]);
            }
            #endregion

            //pagination functionality
            if (page == null || page < 1) page = 1;

            int pageSize = 5;
            int totalPages = 0;

            decimal count = query.Count();
            totalPages = (int)Math.Ceiling(count / pageSize);

            query = query.Skip((int)(page-1)*pageSize).Take(pageSize);

            var products = query.ToList();

            var response = new
            {
                Products = products,
                TotalPages = totalPages,
                Page = page,
                PageSize = pageSize,
            };

            return Ok(response);
        }


        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }


        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult CreateProduct([FromForm]ProductDto productDto)
        {
            if (!listCategories.Contains(productDto.Category))
            {
                ModelState.AddModelError("Category", "Please Select a valid category");
                return BadRequest(ModelState);
            }

            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
                return BadRequest(ModelState);
            }
            //save image on server
            string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            imageFileName += Path.GetExtension(productDto.ImageFile.FileName);

            string imagesFolder = env.WebRootPath + "/images/Products/";
            

            using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            //save product in database 
            Product product = new()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description ?? "",
                ImageFileName = imageFileName,
                CreatedAt = DateTime.Now,
            };
            context.Products.Add(product);
            context.SaveChanges();
            return Ok(product);
        }


        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id,[FromForm]ProductDto productDto)
        {
            if (!listCategories.Contains(productDto.Category))
            {
                ModelState.AddModelError("Category", "Please Select a valid category");
                return BadRequest(ModelState);
            }
            var product = context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            string imageFileName = product.ImageFileName;
            if(productDto.ImageFile != null)
            {
                //save image on server
                imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                imageFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imagesFolder = env.WebRootPath + "/images/Products/";
                using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
                {
                    productDto.ImageFile.CopyTo(stream);
                }
                //delete old image
                System.IO.File.Delete(imageFileName + product.ImageFileName);
            }
            //update product in the database
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Price = productDto.Price;
            product.Category = productDto.Category;
            product.Description = productDto.Description ?? "";
            product.ImageFileName = imageFileName;

            context.SaveChanges();

            return Ok(product);
        }


        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            //delete image from server
            string imagesFolder = env.WebRootPath + "/images/products/";
            System.IO.File.Delete(imagesFolder+product.ImageFileName);

            //delete image from database
            context.Products.Remove(product);
            context.SaveChanges();
            return Ok();
        }
    }
}
