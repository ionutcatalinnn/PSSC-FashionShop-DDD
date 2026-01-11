using FashionShop_Hub.Data.Models;
using static FashionShop.Domain.Models.Entities.Shipping;

namespace FashionShop_Hub.Data.Repositories
{
    public class ShippingRepository
    {
        private readonly FashionDbContext _ctx;
        public ShippingRepository(FashionDbContext ctx) => _ctx = ctx;

        public void Save(ShippedOrder shipping)
        {
            _ctx.Shippings.Add(new ShippingDto
            {
                ShippingId = Guid.NewGuid(),
                OrderId = shipping.OrderId,
                Address = shipping.Address.ToString(),
                AWB = shipping.AWB,
                ShippedAt = shipping.ShippedAt,
                Status = "Shipped"
            });
            _ctx.SaveChanges();
        }
    }
}