using Microsoft.EntityFrameworkCore;
using FashionShop_Hub.Data.Models;

namespace FashionShop_Hub.Data
{
    public class FashionDbContext : DbContext
    {
        public FashionDbContext(DbContextOptions<FashionDbContext> options) : base(options) { }

        public DbSet<OrderDto> Orders { get; set; }
        public DbSet<PaymentDto> Payments { get; set; } // <--- NOU
        public DbSet<ShippingDto> Shippings { get; set; } // <--- NOU

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDto>().ToTable("Orders");
            modelBuilder.Entity<OrderDto>().Property(p => p.TotalAmount).HasColumnType("decimal(18,2)");

            // Configurare Payment
            modelBuilder.Entity<PaymentDto>().ToTable("Payments");
            modelBuilder.Entity<PaymentDto>().Property(p => p.Amount).HasColumnType("decimal(18,2)");

            // Configurare Shipping
            modelBuilder.Entity<ShippingDto>().ToTable("Shippings");
        }
    }
}