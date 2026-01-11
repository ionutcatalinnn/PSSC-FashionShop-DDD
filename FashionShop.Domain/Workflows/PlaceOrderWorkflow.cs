using System.Linq; // <--- IMPORTANT: Adaugă asta pentru .ToList()
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Models.Events;
using FashionShop.Domain.Operations;
using FashionShop.Domain.Repositories;
using static FashionShop.Domain.Models.Entities.Order;
using static FashionShop.Domain.Models.Events.OrderPlacedEvent;

namespace FashionShop.Domain.Workflows
{
    public class PlaceOrderWorkflow
    {
        public IOrderPlacedEvent Execute(PlaceOrderCommand command, IOrderRepository repository)
        {
            // 1. Convertim input-ul în Stare Inițială
            // FIX AICI: Adăugăm .ToList() la command.Lines
            IOrder order = new UnvalidatedOrder(command.Lines.ToList());

            // 2. Executăm pipeline-ul de operații
            
            // Pas A: Validare
            order = new ValidateOrderOperation().Transform(order);
            
            // Pas B: Calculare Preț
            order = new CalculateOrderOperation().Transform(order);
            
            // Pas C: Plasare (Generare ID + Timestamp)
            order = new PlaceOrderFinalOperation().Transform(order);

            // 3. Persistență (Salvare în Bază de Date)
            if (order is PlacedOrder placedOrder)
            {
                // Salvăm entitatea folosind Repository-ul
                repository.Save(placedOrder);
            }

            // 4. Returnăm evenimentul
            return order.ToEvent();
        }
    }
}