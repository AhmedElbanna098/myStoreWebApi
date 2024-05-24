using System.ComponentModel.DataAnnotations;

namespace MyStoreWebAPI.Models
{
    public class UserDto
    {
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty; 

        [Required, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(100), MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
