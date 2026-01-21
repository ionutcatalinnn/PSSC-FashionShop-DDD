using System;
using System.Collections.Generic;
using System.Linq;
using FashionShop.Domain.Models.Entities;
using static FashionShop.Domain.Models.Entities.Order;

namespace FashionShop.Domain.Operations
{
    // Clasa de bază pentru operații (State Machine)
    public abstract class PlaceOrderOperation : DomainOperation<IOrder, object, IOrder>
    {
        public override IOrder Transform(IOrder order, object? state = null) =>
            order switch
            {
                UnvalidatedOrder u => OnUnvalidated(u),
                ValidatedOrder v => OnValidated(v),
                CalculatedOrder c => OnCalculated(c),
                PlacedOrder p => OnPlaced(p),
                InvalidOrder i => OnInvalid(i),
                _ => order
            };

        protected virtual IOrder OnUnvalidated(UnvalidatedOrder order) => order;
        protected virtual IOrder OnValidated(ValidatedOrder order) => order;
        protected virtual IOrder OnCalculated(CalculatedOrder order) => order;
        protected virtual IOrder OnPlaced(PlacedOrder order) => order;
        protected virtual IOrder OnInvalid(InvalidOrder order) => order;
    }

    // 1. Validarea datelor
    public class ValidateOrderOperation : PlaceOrderOperation
    {
        protected override IOrder OnUnvalidated(UnvalidatedOrder order)
        {
            var validatedLines = new List<ValidatedOrderLine>();

            foreach (var line in order.Lines)
            {
                if (line.Quantity <= 0)
                    return new InvalidOrder($"Invalid quantity for {line.ProductCode}");

                // Păstrăm logica ta de creare a liniilor
                validatedLines.Add(new ValidatedOrderLine(line.ProductCode, line.Quantity));
            }

            // --- MODIFICARE AICI ---
            // Pasăm CustomerName și Address din 'order' (Unvalidated) către 'ValidatedOrder'
            return new ValidatedOrder(
                validatedLines, 
                order.CustomerName, 
                order.Address
            );
        }
    }

    // 2. Calculul Prețului
    public class CalculateOrderOperation : PlaceOrderOperation
    {
        protected override IOrder OnValidated(ValidatedOrder order)
        {
            var calculatedLines = new List<CalculatedOrderLine>();
            decimal totalOrder = 0;

            foreach (var line in order.Lines)
            {
                // Simulare preț din baza de date (hardcodat pentru demo)
                decimal price = 50m; 
                decimal lineTotal = price * line.Quantity;

                calculatedLines.Add(new CalculatedOrderLine(line.ProductCode, line.Quantity, price, lineTotal));
                totalOrder += lineTotal;
            }

            // --- MODIFICARE AICI ---
            // Pasăm CustomerName și Address din 'order' (Validated) către 'CalculatedOrder'
            return new CalculatedOrder(
                calculatedLines, 
                totalOrder, 
                order.CustomerName, 
                order.Address
            );
        }
    }

    // 3. Plasarea finală (Generarea ID-ului)
    public class PlaceOrderFinalOperation : PlaceOrderOperation
    {
        protected override IOrder OnCalculated(CalculatedOrder order)
        {
            // --- MODIFICARE AICI ---
            // Pasăm CustomerName și Address din 'order' (Calculated) către 'PlacedOrder'
            // Acestea vor fi folosite ulterior la salvarea în Repository
            return new PlacedOrder(
                OrderId: Guid.NewGuid(),
                Lines: order.Lines, 
                Total: order.Total, 
                PlacedAt: DateTime.UtcNow,
                CustomerName: order.CustomerName, // <--- NOU
                Address: order.Address            // <--- NOU
            );
        }
    }
}