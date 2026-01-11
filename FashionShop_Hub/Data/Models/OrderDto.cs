using System;
using System.ComponentModel.DataAnnotations;

namespace FashionShop_Hub.Data.Models
{
    public class OrderDto
    {
        [Key]
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PlacedAt { get; set; }
        
        // Modificare: Adăugăm = string.Empty;
        public string Status { get; set; } = string.Empty;
        
        // Modificare: Adăugăm = string.Empty;
        public string OrderLinesJson { get; set; } = string.Empty;
    }
}