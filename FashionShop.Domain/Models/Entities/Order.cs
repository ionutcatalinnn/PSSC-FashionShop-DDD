using System;
using System.Collections.Generic;

namespace FashionShop.Domain.Models.Entities
{
    public static class Order
    {
        // (State Machine)
        public interface IOrder { }

        // 1. Starea Inițială: Comanda nevalidată 
        // AM ADAUGAT: CustomerName si Address
        public record UnvalidatedOrder(List<UnvalidatedOrderLine> Lines, string CustomerName, string Address) : IOrder;

        // 2. Starea de Eroare: Ceva nu a fost bine
        public record InvalidOrder(string Reason) : IOrder;

        // 3. Starea Validată: Produsele există și cantitățile sunt ok
        // AM ADAUGAT: CustomerName si Address (le pasam mai departe)
        public record ValidatedOrder(List<ValidatedOrderLine> Lines, string CustomerName, string Address) : IOrder;

        // 4. Starea Calculată: S-a calculat prețul total
        // AM ADAUGAT: CustomerName si Address
        public record CalculatedOrder(List<CalculatedOrderLine> Lines, decimal Total, string CustomerName, string Address) : IOrder;

        // 5. Starea Finală: PLASATĂ CU SUCCES
        // AM ADAUGAT: CustomerName si Address (ca sa le salvam in baza)
        public record PlacedOrder(Guid OrderId, List<CalculatedOrderLine> Lines, decimal Total, DateTime PlacedAt, string CustomerName, string Address) : IOrder;
    }
}