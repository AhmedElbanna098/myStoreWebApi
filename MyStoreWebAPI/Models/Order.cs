﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyStoreWebAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        [Precision(16,2)]
        public decimal ShippingFee { get; set; }

        [MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";

        [MaxLength(100)]
        public string PaymentMethod { get; set; } = "";

        [MaxLength(100)]
        public string PaymentStatus { get; set; } = "";

        [MaxLength(100)]
        public string OrderStatus { get; set; } = "";

        //navigation properties
        public User User { get; set; } = null!;

        public List<OrderItem> Items { get; set; } = new();

    }
}