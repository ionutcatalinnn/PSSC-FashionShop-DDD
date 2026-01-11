using System;
using System.ComponentModel.DataAnnotations;

namespace FashionShop_Hub.Data.Models
{
    public class ShippingDto
    {
        [Key]
        public Guid ShippingId { get; set; }
        public Guid OrderId { get; set; }
        
        // Modificări aici:
        public string Address { get; set; } = string.Empty;
        public string AWB { get; set; } = string.Empty;
        public DateTime ShippedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}