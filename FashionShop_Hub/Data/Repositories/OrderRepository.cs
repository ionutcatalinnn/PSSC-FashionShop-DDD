using System.Text.Json;
using FashionShop_Hub.Data;          // <--- AICI ERA PROBLEMA (Lipseal acest using)
using FashionShop_Hub.Data.Models;  // Aici e OrderDto
using FashionShop.Domain.Repositories;
using static FashionShop.Domain.Models.Entities.Order;

namespace FashionShop.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        // Acum va recunoaste clasa FashionDbContext
        private readonly FashionDbContext _dbContext;

        public OrderRepository(FashionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Save(PlacedOrder order)
        {
            var orderEntity = new OrderDto
            {
                OrderId = order.OrderId,
                TotalAmount = order.Total,
                PlacedAt = order.PlacedAt,
                Status = "Placed",
                
                // Salvam datele clientului
                CustomerName = order.CustomerName,
                Address = order.Address,

                // Serializam liniile
                OrderLinesJson = JsonSerializer.Serialize(order.Lines)
            };

            _dbContext.Orders.Add(orderEntity);
            _dbContext.SaveChanges();
        }
    }
}