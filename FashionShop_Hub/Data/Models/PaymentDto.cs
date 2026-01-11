using System;
using System.ComponentModel.DataAnnotations;

namespace FashionShop_Hub.Data.Models
{
    public class PaymentDto
    {
        [Key]
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ProcessedAt { get; set; }
        
        // Modificare: Adăugăm = string.Empty;
        public string Status { get; set; } = string.Empty; 
    }
}