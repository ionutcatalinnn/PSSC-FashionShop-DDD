using System;
using System.Collections.Generic;

namespace FashionShop.Domain.Models.Entities
{
    public static class Order
    {
        public interface IOrder { }

        // --- DEFINEȘTE LINIILE AICI ---
        public record UnvalidatedOrderLine(string ProductCode, int Quantity, decimal Price);
        public record ValidatedOrderLine(string ProductCode, int Quantity, decimal Price);
        public record CalculatedOrderLine(string ProductCode, int Quantity, decimal Price, decimal FinalPrice);

        // --- DEFINEȘTE STĂRILE ---
        public record UnvalidatedOrder(List<UnvalidatedOrderLine> Lines, string CustomerName, string Address) : IOrder;
        public record ValidatedOrder(List<ValidatedOrderLine> Lines, string CustomerName, string Address) : IOrder;
        public record CalculatedOrder(List<CalculatedOrderLine> Lines, decimal Total, string CustomerName, string Address) : IOrder;
        public record PlacedOrder(Guid OrderId, List<CalculatedOrderLine> Lines, decimal Total, DateTime PlacedAt, string CustomerName, string Address) : IOrder;
        public record InvalidOrder(string Reason) : IOrder;
    }
}