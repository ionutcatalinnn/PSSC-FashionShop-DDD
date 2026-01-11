using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using FashionShop.Domain; // Pentru IAsyncEventBus
using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Workflows;
using FashionShop_Hub.Data.Repositories;
using static FashionShop.Domain.Models.Events.OrderPlacedEvent;
using static FashionShop.Domain.Models.Events.PaymentProcessedEvent;
using FashionShop.Domain.Models.ValueObjects;

namespace FashionShop_Hub.BackgroundServices
{
    public class PaymentWorker : BackgroundService
    {
        private readonly IAsyncEventBus _eventBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentWorker> _logger;

        public PaymentWorker(IAsyncEventBus eventBus, IServiceProvider serviceProvider, ILogger<PaymentWorker> logger)
        {
            _eventBus = eventBus;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Ascultăm mesajele din Azure
            await foreach (var orderEvent in _eventBus.SubscribeAsync<OrderPlacedSuccessfully>(stoppingToken))
            {
                _logger.LogInformation($"⚡ AZURE WORKER: Order {orderEvent.OrderId} received. Processing payment...");

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var paymentRepo = scope.ServiceProvider.GetRequiredService<PaymentRepository>();
                        
                        // 1. Facem comanda de plată
                        var payCommand = new PayOrderCommand(
                            orderEvent.OrderId.ToString(), 
                            orderEvent.Total, 
                            "1234123412341234", 
                            "123");

                        // 2. Executăm Workflow-ul
                        var workflow = new ProcessPaymentWorkflow();
                        var result = workflow.Execute(payCommand);

                        // 3. Salvăm și trimitem confirmarea înapoi în Azure
                        if (result is PaymentSucceeded paymentSuccess)
                        {
                            paymentRepo.Save(new FashionShop.Domain.Models.Entities.Payment.ProcessedPayment(
                                paymentSuccess.PaymentId, 
                                paymentSuccess.OrderId, 
                                new Price(orderEvent.Total), 
                                paymentSuccess.ProcessedAt));

                            _logger.LogInformation($"✅ AZURE WORKER: Payment Success! Publishing event...");
                            
                            await _eventBus.PublishAsync(paymentSuccess);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Payment Error: {ex.Message}");
                }
            }
        }
    }
}