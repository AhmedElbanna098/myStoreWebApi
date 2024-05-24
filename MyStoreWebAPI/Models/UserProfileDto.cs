﻿using System.ComponentModel.DataAnnotations;

namespace MyStoreWebAPI.Models
{
    // data to send confirmation to user from server
    public class UserProfileDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty; 

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
