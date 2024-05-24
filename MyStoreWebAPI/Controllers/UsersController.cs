using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStoreWebAPI.Models;
using MyStoreWebAPI.Services;

namespace MyStoreWebAPI.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly AppDbContext context;

        public UsersController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetUsers(int? page)
        {
            if (page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 5;
            int totalPages = 0;

            decimal count = context.Users.Count();
            totalPages = (int)Math.Ceiling(count/pageSize);

            var users = context.Users
                .OrderByDescending(x => x.Id)
                .Skip((int)(page-1)*pageSize)
                .Take(pageSize)
                .ToList();
            
            List<UserProfileDto> userProfiles = new List<UserProfileDto>();
            foreach (var user in users)
            {
                var userProfileDto = new UserProfileDto()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Email = user.Email,
                    Address = user.Address,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                };
                userProfiles.Add(userProfileDto);
            }

            var response = new
            {
                Users = userProfiles,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            var userProfileDto = new UserProfileDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
            };

            return Ok(userProfileDto);

        }
    }
}
