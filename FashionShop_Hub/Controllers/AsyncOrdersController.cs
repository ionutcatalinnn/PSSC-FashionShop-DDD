using Microsoft.AspNetCore.Mvc;
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Workflows;
using FashionShop.Domain.Repositories;
using FashionShop.Domain; // Pentru IAsyncEventBus
using FashionShop.Domain.Models.Events; // <--- Aici este OrderPlacedEvent acum

namespace FashionShop_Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsyncOrdersController : ControllerBase
    {
        // Workflow-ul nu mai este injectat prin DI, îl instanțiem direct sau îl înregistrăm simplu
        // Dar pentru simplitate acum, îl folosim direct.
        private readonly PlaceOrderWorkflow _workflow; 
        private readonly IOrderRepository _repository;
        private readonly IAsyncEventBus _eventBus;

        public AsyncOrdersController(IOrderRepository repository, IAsyncEventBus eventBus)
        {
            _repository = repository;
            _eventBus = eventBus;
            _workflow = new PlaceOrderWorkflow();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] PlaceOrderCommand command)
        {
            try
            {
                // 1. Executăm Workflow-ul
                // Acum returnează DIRECT "OrderPlacedEvent". Nu mai trebuie să verifici "is OrderPlacedSuccessfully".
                var eventResult = _workflow.Execute(command, _repository);

                // 2. Trimitem evenimentul către Azure Service Bus
                await _eventBus.PublishAsync(eventResult);

                // 3. Returnăm succes
                return Ok(new 
                { 
                    Message = "Comanda a fost procesată și trimisă spre livrare!", 
                    OrderId = eventResult.OrderId,
                    Total = eventResult.Total
                });
            }
            catch (Exception ex)
            {
                // Dacă workflow-ul eșuează (validare, stoc, etc.), va arunca o eroare pe care o prindem aici
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}