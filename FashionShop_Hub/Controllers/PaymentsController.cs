using Microsoft.AspNetCore.Mvc;
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Workflows;
using static FashionShop.Domain.Models.Events.PaymentProcessedEvent;
using FashionShop_Hub.Data.Repositories;
using static FashionShop.Domain.Models.Entities.Payment;

namespace FashionShop_Hub.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentRepository _repo; // <--- Injectat

    public PaymentsController(PaymentRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    public IActionResult PayOrder([FromBody] PayOrderCommand command)
    {
        var workflow = new ProcessPaymentWorkflow();
        var result = workflow.Execute(command);

        // SALVARE
        if (result is PaymentSucceeded success)
        {
            // Reconstruim obiectul pentru salvare (sau modificăm workflow să returneze entity)
            // Varianta rapidă: folosim datele din succes event
            _repo.Save(new ProcessedPayment(success.PaymentId, success.OrderId, new FashionShop.Domain.Models.ValueObjects.Price(command.Amount), success.ProcessedAt));
        }

        return result switch
        {
            PaymentSucceeded s => Ok(s),
            PaymentFailed f => BadRequest(f),
            _ => StatusCode(500, "Error")
        };
    }
}