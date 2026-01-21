using Microsoft.AspNetCore.Mvc;
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Workflows;
using FashionShop.Domain.Repositories;
using FashionShop.Domain.Models.Events; // <--- Aici este definit OrderPlacedEvent
using FashionShop.Domain; // Pentru IAsyncEventBus

namespace FashionShop_Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly IAsyncEventBus _eventBus;

        public OrdersController(IOrderRepository repository, IAsyncEventBus eventBus)
        {
            _repository = repository;
            _eventBus = eventBus;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] PlaceOrderCommand command)
        {
            try
            {
                // Instanțiem workflow-ul
                var workflow = new PlaceOrderWorkflow();

                // 1. Executăm Workflow-ul
                // ACUM returnează direct 'OrderPlacedEvent', nu mai trebuie să verifici tipul
                OrderPlacedEvent orderEvent = workflow.Execute(command, _repository);

                // 2. Trimitem evenimentul către Azure Service Bus
                // (Dacă folosești Async, altfel poți comenta linia asta)
                if (_eventBus != null)
                {
                    await _eventBus.PublishAsync(orderEvent);
                }

                // 3. Returnăm succes cu datele din eveniment
                return Ok(new 
                { 
                    Message = "Comanda a fost plasată cu succes!", 
                    OrderId = orderEvent.OrderId,
                    Total = orderEvent.Total,
                    Date = orderEvent.PlacedAt
                });
            }
            catch (Exception ex)
            {
                // Dacă workflow-ul dă eroare (validare etc.), returnăm 400 Bad Request
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}