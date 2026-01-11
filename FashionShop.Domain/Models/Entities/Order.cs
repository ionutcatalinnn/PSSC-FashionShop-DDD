using System;
using System.Collections.Generic;

namespace FashionShop.Domain.Models.Entities
{
    public static class Order
    {
        //  (State Machine)
        public interface IOrder { }

        // 1. Starea Inițială: Comanda nevalidată 
        public record UnvalidatedOrder(List<UnvalidatedOrderLine> Lines) : IOrder;

        // 2. Starea de Eroare: Ceva nu a fost bine
        public record InvalidOrder(string Reason) : IOrder;

        // 3. Starea Validată: Produsele există și cantitățile sunt ok
        public record ValidatedOrder(List<ValidatedOrderLine> Lines) : IOrder;

        // 4. Starea Calculată: S-a calculat prețul total
        public record CalculatedOrder(List<CalculatedOrderLine> Lines, decimal Total) : IOrder;

        // 5. Starea Finală: PLASATĂ CU SUCCES
        
        
        public record PlacedOrder(Guid OrderId, List<CalculatedOrderLine> Lines, decimal Total, DateTime PlacedAt) : IOrder;
    }
}