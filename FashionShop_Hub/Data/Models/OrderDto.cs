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
        
        public string Status { get; set; } = string.Empty;
        public string OrderLinesJson { get; set; } = string.Empty;

        // --- AM ADAUGAT ACESTE DOUA LINII ---
        // Acum baza de date va avea coloane pentru ele
        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}