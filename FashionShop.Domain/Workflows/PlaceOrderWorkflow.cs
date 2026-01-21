using System.Linq; // <--- Necesar pentru .Select() si .ToList()
using System.Collections.Generic;
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Models.Events;
using FashionShop.Domain.Operations;
using FashionShop.Domain.Repositories;
using FashionShop.Domain.Models.Entities; // Asigura-te ca ai acest namespace
using static FashionShop.Domain.Models.Entities.Order;
using static FashionShop.Domain.Models.Events.OrderPlacedEvent;

namespace FashionShop.Domain.Workflows
{
    public class PlaceOrderWorkflow
    {
        public IOrderPlacedEvent Execute(PlaceOrderCommand command, IOrderRepository repository)
        {
            // 1. Convertim (Mapăm) liniile de la INPUT la DOMAIN
            // Aici era eroarea: transformăm OrderLineInput -> UnvalidatedOrderLine
            var domainLines = command.Lines
                .Select(line => new UnvalidatedOrderLine(line.ProductCode, line.Quantity))
                .ToList();

            // 2. Creăm starea inițială cu lista convertită
            IOrder order = new UnvalidatedOrder(
                domainLines, 
                command.CustomerName, 
                command.Address
            );

            // 3. Executăm pipeline-ul de operații
            
            // Pas A: Validare
            order = new ValidateOrderOperation().Transform(order);
            
            // Pas B: Calculare Preț
            order = new CalculateOrderOperation().Transform(order);
            
            // Pas C: Plasare (Generare ID + Timestamp)
            order = new PlaceOrderFinalOperation().Transform(order);

            // 4. Persistență (Salvare în Bază de Date)
            if (order is PlacedOrder placedOrder)
            {
                repository.Save(placedOrder);
            }

            // 5. Returnăm evenimentul
            return order.ToEvent();
        }
    }
}