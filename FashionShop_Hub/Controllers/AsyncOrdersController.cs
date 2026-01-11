using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FashionShop.Domain; // Pentru IAsyncEventBus
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Workflows;
using FashionShop.Domain.Repositories;
using static FashionShop.Domain.Models.Events.OrderPlacedEvent;

namespace FashionShop_Hub.Controllers
{
    [ApiController]
    [Route("api/async-orders")]
    public class AsyncOrdersController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly IAsyncEventBus _eventBus;

        public AsyncOrdersController(IOrderRepository repository, IAsyncEventBus eventBus)
        {
            _repository = repository;
            _eventBus = eventBus;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrderAsync([FromBody] PlaceOrderCommand command)
        {
            var workflow = new PlaceOrderWorkflow();
            var result = workflow.Execute(command, _repository);

            if (result is OrderPlacedSuccessfully successEvent)
            {
                // Trimitem în Azure Service Bus
                await _eventBus.PublishAsync(successEvent);

                return Accepted(new { 
                    Message = "Comanda a fost trimisă în Azure Queue!", 
                    OrderId = successEvent.OrderId 
                });
            }

            return BadRequest("Comanda invalidă.");
        }
    }
}