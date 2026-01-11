using System.Text.Json;
using FashionShop.Domain.Models.Entities;
using FashionShop.Domain.Repositories;
using FashionShop_Hub.Data.Models;
using static FashionShop.Domain.Models.Entities.Order;

namespace FashionShop_Hub.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly FashionDbContext _dbContext;

        public OrderRepository(FashionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Save(PlacedOrder order)
        {
            // Convertim din Domain Entity (PlacedOrder) în Database Entity (OrderDto)
            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                TotalAmount = order.Total,
                PlacedAt = order.PlacedAt,
                Status = "Placed",
                
                // Serializăm liniile ca JSON pentru simplitate
                // Aici nu avem nevoie de .Value pentru că ProductCode este deja string
                OrderLinesJson = JsonSerializer.Serialize(order.Lines)
            };

            _dbContext.Orders.Add(orderDto);
            _dbContext.SaveChanges();
        }
    }
}