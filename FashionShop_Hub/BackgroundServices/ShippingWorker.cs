using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using FashionShop.Domain;
using FashionShop.Domain.Models.Events; // Verifică să ai OrderPaidEvent aici
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Workflows;
using FashionShop_Hub.Data.Repositories;
using FashionShop.Domain.Models.ValueObjects;
using static FashionShop.Domain.Models.Events.OrderShippedEvent;

namespace FashionShop_Hub.BackgroundServices
{
    public class ShippingWorker : BackgroundService
    {
        private readonly IAsyncEventBus _eventBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ShippingWorker> _logger;

        public ShippingWorker(IAsyncEventBus eventBus, IServiceProvider serviceProvider, ILogger<ShippingWorker> logger)
        {
            _eventBus = eventBus;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 SHIPPING WORKER: Pornit și ascultă coada 'payments'...");

            // Ne abonăm la OrderPaidEvent (ceea ce trimite PaymentWorker)
            await foreach (var paymentEvent in _eventBus.SubscribeAsync<OrderPaidEvent>(stoppingToken))
            {
                _logger.LogInformation($"⚡ PLATA RECEPȚIONATĂ: Pregătesc livrarea pentru comanda {paymentEvent.OrderId}");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var shippingRepo = scope.ServiceProvider.GetRequiredService<ShippingRepository>();

                    // 1. Executăm logica de business (Workflow)
                    var shipCommand = new ShipOrderCommand(paymentEvent.OrderId.ToString(), "Timisoara", "Centru");
                    var workflow = new ShipOrderWorkflow();
                    var result = workflow.Execute(shipCommand);

                    if (result is ShippingSucceeded shipSuccess)
                    {
                        // 2. Salvăm în baza de date folosind Repository-ul tău cu EF
                        shippingRepo.Save(new FashionShop.Domain.Models.Entities.Shipping.ShippedOrder(
                            shipSuccess.OrderId, 
                            new ShippingAddress("Timisoara", "Centru"), 
                            shipSuccess.AWB, 
                            shipSuccess.ShippedAt));

                        _logger.LogInformation($"📦 LIVRARE FINALIZATĂ: AWB generat: {shipSuccess.AWB}");
                    }
                }
            }
        }
    }
}