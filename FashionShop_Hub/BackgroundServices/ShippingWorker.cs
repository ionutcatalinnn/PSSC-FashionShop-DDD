using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using FashionShop.Domain; // Pentru IAsyncEventBus
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Workflows;
using FashionShop_Hub.Data.Repositories;
using static FashionShop.Domain.Models.Events.PaymentProcessedEvent;
using static FashionShop.Domain.Models.Events.OrderShippedEvent;
using FashionShop.Domain.Models.ValueObjects;

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
            await foreach (var paymentEvent in _eventBus.SubscribeAsync<PaymentSucceeded>(stoppingToken))
            {
                _logger.LogInformation($"⚡ AZURE WORKER: Payment verified for Order {paymentEvent.OrderId}. Shipping...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var shippingRepo = scope.ServiceProvider.GetRequiredService<ShippingRepository>();

                    var shipCommand = new ShipOrderCommand(paymentEvent.OrderId.ToString(), "Timisoara", "Centru");
                    
                    var workflow = new ShipOrderWorkflow();
                    var result = workflow.Execute(shipCommand);

                    if (result is ShippingSucceeded shipSuccess)
                    {
                        shippingRepo.Save(new FashionShop.Domain.Models.Entities.Shipping.ShippedOrder(
                            shipSuccess.OrderId, 
                            new ShippingAddress("Timisoara", "Centru"), 
                            shipSuccess.AWB, 
                            shipSuccess.ShippedAt));

                        _logger.LogInformation($"📦 AZURE WORKER: Order Shipped! AWB: {shipSuccess.AWB}");
                    }
                }
            }
        }
    }
}