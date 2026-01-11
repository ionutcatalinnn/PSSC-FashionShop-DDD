using System;
using static FashionShop.Domain.Models.Entities.Shipping;

namespace FashionShop.Domain.Models.Events
{
    public static class OrderShippedEvent
    {
        public interface IOrderShippedEvent { }

        public record ShippingSucceeded(Guid OrderId, string AWB, DateTime ShippedAt) : IOrderShippedEvent;

        public record ShippingFailed(string Reason) : IOrderShippedEvent;

        public static IOrderShippedEvent ToEvent(this IShipping shipping) =>
            shipping switch
            {
                ShippedOrder s => new ShippingSucceeded(s.OrderId, s.AWB, s.ShippedAt),
                InvalidShipping i => new ShippingFailed(i.Reason),
                _ => new ShippingFailed("Unexpected shipping state")
            };
    }
}