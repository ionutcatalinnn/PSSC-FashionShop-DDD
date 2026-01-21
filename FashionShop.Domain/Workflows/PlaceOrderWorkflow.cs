using System;
using System.Linq;
using System.Collections.Generic;
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Models.Events; // Aici e OrderPlacedEvent
using FashionShop.Domain.Operations;
using FashionShop.Domain.Repositories;
using FashionShop.Domain.Models.Entities; 

namespace FashionShop.Domain.Workflows
{
    public class PlaceOrderWorkflow
    {
        // MODIFICARE: Returnăm 'OrderPlacedEvent' (concret), nu 'IOrderPlacedEvent'
        public OrderPlacedEvent Execute(PlaceOrderCommand command, IOrderRepository repository)
        {
            // 1. Convertim inputul în liniile de domeniu (Unvalidated)
            var domainLines = command.Lines
                .Select(line => new Order.UnvalidatedOrderLine(line.ProductCode, line.Quantity, line.Price))
                .ToList();

            // 2. Creăm starea inițială
            Order.IOrder order = new Order.UnvalidatedOrder(
                domainLines, 
                command.CustomerName, 
                command.Address
            );

            // 3. Executăm pipeline-ul
            order = new ValidateOrderOperation().Transform(order);
            order = new CalculateOrderOperation().Transform(order);
            order = new PlaceOrderFinalOperation().Transform(order);

            // 4. Verificăm rezultatul și salvăm
            if (order is Order.PlacedOrder placedOrder)
            {
                // Salvăm în baza de date
                repository.Save(placedOrder);

                // Returnăm evenimentul concret
                return new OrderPlacedEvent(placedOrder.OrderId, placedOrder.Total, placedOrder.PlacedAt);
            }

            // Dacă ceva a eșuat
            throw new Exception("Workflow failed: Order could not be placed.");
        }
    }
}