using System;
using System.Collections.Generic;
using System.Linq;
using FashionShop.Domain.Models.Entities;
// ATENȚIE: Am scos 'using static ... Order' ca să te oblig să folosești prefixul 'Order.'
// Așa nu mai apar confuzii.

namespace FashionShop.Domain.Operations
{
    // State Machine
    public abstract class PlaceOrderOperation : DomainOperation<Order.IOrder, object, Order.IOrder>
    {
        public override Order.IOrder Transform(Order.IOrder order, object? state = null) =>
            order switch
            {
                Order.UnvalidatedOrder u => OnUnvalidated(u),
                Order.ValidatedOrder v => OnValidated(v),
                Order.CalculatedOrder c => OnCalculated(c),
                Order.PlacedOrder p => OnPlaced(p),
                Order.InvalidOrder i => OnInvalid(i),
                _ => order
            };

        protected virtual Order.IOrder OnUnvalidated(Order.UnvalidatedOrder order) => order;
        protected virtual Order.IOrder OnValidated(Order.ValidatedOrder order) => order;
        protected virtual Order.IOrder OnCalculated(Order.CalculatedOrder order) => order;
        protected virtual Order.IOrder OnPlaced(Order.PlacedOrder order) => order;
        protected virtual Order.IOrder OnInvalid(Order.InvalidOrder order) => order;
    }

    // 1. Validarea
    public class ValidateOrderOperation : PlaceOrderOperation
    {
        protected override Order.IOrder OnUnvalidated(Order.UnvalidatedOrder order)
        {
            var validatedLines = new List<Order.ValidatedOrderLine>(); // <--- Prefix explicit Order.

            foreach (var line in order.Lines)
            {
                if (line.Quantity <= 0)
                    return new Order.InvalidOrder($"Invalid quantity for {line.ProductCode}");

                // Aici folosim explicit Order.ValidatedOrderLine
                validatedLines.Add(new Order.ValidatedOrderLine(line.ProductCode, line.Quantity, line.Price));
            }

            return new Order.ValidatedOrder(validatedLines, order.CustomerName, order.Address);
        }
    }

    // 2. Calculul
    public class CalculateOrderOperation : PlaceOrderOperation
    {
        protected override Order.IOrder OnValidated(Order.ValidatedOrder order)
        {
            var calculatedLines = new List<Order.CalculatedOrderLine>();
            decimal totalOrder = 0;

            foreach (var line in order.Lines)
            {
                decimal price = line.Price;
                decimal lineTotal = price * line.Quantity;

                calculatedLines.Add(new Order.CalculatedOrderLine(line.ProductCode, line.Quantity, price, lineTotal));
                totalOrder += lineTotal;
            }

            return new Order.CalculatedOrder(calculatedLines, totalOrder, order.CustomerName, order.Address);
        }
    }

    // 3. Plasarea
    public class PlaceOrderFinalOperation : PlaceOrderOperation
    {
        protected override Order.IOrder OnCalculated(Order.CalculatedOrder order)
        {
            return new Order.PlacedOrder(
                OrderId: Guid.NewGuid(),
                Lines: order.Lines, 
                Total: order.Total, 
                PlacedAt: DateTime.UtcNow,
                CustomerName: order.CustomerName,
                Address: order.Address
            );
        }
    }
}