using Microsoft.AspNetCore.Mvc;
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Workflows;
using static FashionShop.Domain.Models.Events.OrderShippedEvent;
using FashionShop_Hub.Data.Repositories;
using static FashionShop.Domain.Models.Entities.Shipping;
namespace FashionShop_Hub.Controllers;

[ApiController]
[Route("api/shipping")]
public class ShippingController : ControllerBase
{
    private readonly ShippingRepository _repo;

    public ShippingController(ShippingRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    public IActionResult ShipOrder([FromBody] ShipOrderCommand command)
    {
        var workflow = new ShipOrderWorkflow();
        var result = workflow.Execute(command);

        // SALVARE
        if (result is ShippingSucceeded success)
        {
            // Atenție la reconstrucție:
            var address = new FashionShop.Domain.Models.ValueObjects.ShippingAddress(command.City, command.Street);
            _repo.Save(new ShippedOrder(success.OrderId, address, success.AWB, success.ShippedAt));
        }

        return result switch
        {
            ShippingSucceeded s => Ok(s),
            ShippingFailed f => BadRequest(f),
            _ => StatusCode(500, "Error")
        };
    }
}