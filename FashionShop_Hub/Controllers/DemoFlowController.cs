/*using Microsoft.AspNetCore.Mvc;
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Workflows;
using FashionShop.Domain.Repositories;
using FashionShop_Hub.Data.Repositories;
using FashionShop.Domain.Models.ValueObjects;
using static FashionShop.Domain.Models.Events.OrderPlacedEvent;
using static FashionShop.Domain.Models.Events.PaymentProcessedEvent;
using static FashionShop.Domain.Models.Events.OrderShippedEvent;
using System.Text;

namespace FashionShop_Hub.Controllers
{
    // Model special doar pentru acest demo - primește TOT
    // Model special doar pentru acest demo - primește TOT
    public class CompleteFlowRequest
    {
        // Inițializăm lista ca să nu fie null
        public List<FashionShop.Domain.Models.Entities.UnvalidatedOrderLine> Lines { get; set; } = new();
        
        public decimal Amount { get; set; } // decimal are implicit valoarea 0, deci e ok
        
        // Inițializăm string-urile cu gol
        public string CardNumber { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/demo")]
    public class DemoFlowController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly PaymentRepository _paymentRepo;
        private readonly ShippingRepository _shippingRepo;

        public DemoFlowController(IOrderRepository orderRepo, PaymentRepository paymentRepo, ShippingRepository shippingRepo)
        {
            _orderRepo = orderRepo;
            _paymentRepo = paymentRepo;
            _shippingRepo = shippingRepo;
        }

        [HttpPost("run-complete-cycle")]
        public IActionResult RunCompleteFlow([FromBody] CompleteFlowRequest request)
        {
            var log = new StringBuilder();
            log.AppendLine("🚀 STARTING COMPLETE FLOW DEMO...");

        
            // 1. PLASARE COMANDĂ (Order Workflow)
            
            log.AppendLine("1️⃣ Executing PlaceOrderWorkflow...");
            var orderWorkflow = new PlaceOrderWorkflow();
            var orderCommand = new PlaceOrderCommand(request.Lines);
            
            var orderResult = orderWorkflow.Execute(orderCommand, _orderRepo);

            if (orderResult is not OrderPlacedSuccessfully orderSuccess)
            {
                return BadRequest($"❌ Order Failed: {orderResult}");
            }
            
            // Verificăm prin reflection sau presupunem că ai adăugat proprietatea:
            Guid orderId = Guid.Empty;
            try { orderId = (Guid)((dynamic)orderSuccess).OrderId; } 
            catch { orderId = Guid.NewGuid(); log.AppendLine("⚠️ (Warning: OrderId missing in Event, generated dummy)"); }

            log.AppendLine($"✅ Order Placed! ID: {orderId}");

            
            // 2. PROCESARE PLATĂ (Payment Workflow)
            
            log.AppendLine("2️⃣ Executing ProcessPaymentWorkflow...");
            var payWorkflow = new ProcessPaymentWorkflow();
            var payCommand = new PayOrderCommand(orderId.ToString(), request.Amount, request.CardNumber, request.Cvv);
            
            var payResult = payWorkflow.Execute(payCommand);

            if (payResult is not PaymentSucceeded paySuccess)
            {
                return BadRequest($"❌ Payment Failed: {payResult}");
            }

            // Salvare explicită Payment (că workflow-ul de plată nu are repo injectat în codul anterior)
            _paymentRepo.Save(new FashionShop.Domain.Models.Entities.Payment.ProcessedPayment(
                paySuccess.PaymentId, paySuccess.OrderId, new Price(request.Amount), paySuccess.ProcessedAt));

            log.AppendLine($"✅ Payment Processed! ID: {paySuccess.PaymentId}");

            
            // 3. EXPEDIERE (Shipping Workflow)
            log.AppendLine("3️⃣ Executing ShipOrderWorkflow...");
            var shipWorkflow = new ShipOrderWorkflow();
            var shipCommand = new ShipOrderCommand(orderId.ToString(), request.City, request.Street);

            var shipResult = shipWorkflow.Execute(shipCommand);

            if (shipResult is not ShippingSucceeded shipSuccess)
            {
                return BadRequest($"❌ Shipping Failed: {shipResult}");
            }

            // Salvare explicită Shipping
            _shippingRepo.Save(new FashionShop.Domain.Models.Entities.Shipping.ShippedOrder(
                shipSuccess.OrderId, new ShippingAddress(request.City, request.Street), shipSuccess.AWB, shipSuccess.ShippedAt));

            log.AppendLine($"✅ Order Shipped! AWB: {shipSuccess.AWB}");
            log.AppendLine("🎉 COMPLETE FLOW FINISHED SUCCESSFULLY!");

            return Ok(new { 
                Message = "Flow executed successfully", 
                Log = log.ToString(),
                FinalData = new { OrderId = orderId, AWB = shipSuccess.AWB }
            });
        }
    }
}
*/