using Microsoft.AspNetCore.Mvc;
using FashionShop_Hub.Data.Repositories;
using System;

namespace FashionShop_Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentRepository _repository;

        public PaymentsController(PaymentRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult CreatePayment([FromBody] PaymentRequest request)
        {
            try
            {
                // AICI ERA EROAREA:
                // Înainte apelai .Save(), acum apelăm metoda corectă .SavePayment()
                _repository.SavePayment(request.OrderId, request.Amount);

                return Ok(new { Message = "Plata a fost înregistrată cu succes!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    // Definim un model simplu pentru datele care vin din Postman/Frontend
    // (Poți să-l muți într-un fișier separat dacă vrei, dar merge și aici)
    public record PaymentRequest(Guid OrderId, decimal Amount);
}