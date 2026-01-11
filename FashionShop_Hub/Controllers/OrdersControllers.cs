using Microsoft.AspNetCore.Mvc;
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Workflows;
using FashionShop.Domain.Repositories; // <--- using nou
using static FashionShop.Domain.Models.Events.OrderPlacedEvent;

namespace FashionShop_Hub.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly IOrderRepository _repository; // <--- Dependința

    // Constructorul primește repository-ul (Dependency Injection)
    public OrdersController(ILogger<OrdersController> logger, IOrderRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpPost]
    public IActionResult PlaceOrder([FromBody] PlaceOrderCommand command)
    {
        var workflow = new PlaceOrderWorkflow();
        
        // Pasăm repository-ul către workflow
        var result = workflow.Execute(command, _repository);

        return result switch
        {
            OrderPlacedSuccessfully success => Ok(success),
            OrderPlacementFailed failed => BadRequest(failed),
            _ => StatusCode(500, "Unexpected error")
        };
    }
}