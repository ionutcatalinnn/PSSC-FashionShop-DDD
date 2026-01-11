using System;
using FashionShop.Domain.Models.Entities;
using static FashionShop.Domain.Models.Entities.Order;

namespace FashionShop.Domain.Models.Events
{
    public static class OrderPlacedEvent
    {
        public interface IOrderPlacedEvent { }

        // --- MODIFICARE AICI: Am adăugat Guid OrderId ---
        public record OrderPlacedSuccessfully(Guid OrderId, decimal Total, DateTime PlacedAt) : IOrderPlacedEvent;

        public record OrderPlacementFailed(string Reason) : IOrderPlacedEvent;

        public static IOrderPlacedEvent ToEvent(this IOrder order) =>
            order switch
            {
               
                PlacedOrder p => new OrderPlacedSuccessfully(p.OrderId, p.Total, p.PlacedAt),
                
                InvalidOrder i => new OrderPlacementFailed(i.Reason),
                _ => new OrderPlacementFailed("Unexpected order state")
            };
    }
}