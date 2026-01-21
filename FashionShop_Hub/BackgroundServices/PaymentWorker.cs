using Azure.Messaging.ServiceBus;
using FashionShop.Domain;
using FashionShop.Domain.Models.Events;
using FashionShop_Hub.Data.Repositories;
using System.Text.Json; // <--- Aici e schimbarea (Standard .NET)
using System.Text;

namespace FashionShop_Hub.BackgroundServices
{
    public class PaymentWorker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private ServiceBusProcessor _processor;

        public PaymentWorker(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionString = _configuration.GetConnectionString("AzureServiceBus");
            var client = new ServiceBusClient(connectionString);

            _processor = client.CreateProcessor("orders", new ServiceBusProcessorOptions());

            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;

            await _processor.StartProcessingAsync(stoppingToken);
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            try 
            {
                string body = args.Message.Body.ToString();
                var orderEvent = JsonSerializer.Deserialize<OrderPlacedEvent>(body);

                if (orderEvent != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var paymentRepo = scope.ServiceProvider.GetRequiredService<PaymentRepository>();
                        var eventBus = scope.ServiceProvider.GetRequiredService<IAsyncEventBus>();

                        Console.WriteLine($"💰 PAYMENT WORKER: Procesez plata pentru Comanda {orderEvent.OrderId}...");

                        paymentRepo.SavePayment(orderEvent.OrderId, orderEvent.Total);
                        Console.WriteLine("✅ PLATA REUȘITĂ și salvată în DB!");

                        // Dacă aceasta linie crapă, va sări direct la 'catch' 
                        // și va ignora 'CompleteMessageAsync' de mai jos
                        await eventBus.PublishAsync(new OrderPaidEvent(orderEvent.OrderId, orderEvent.Total, DateTime.UtcNow));
                
                        Console.WriteLine("🚚 Evenimentul 'OrderPaid' a fost trimis către Shipping!");
                    }
                }

                // ACEASTA ESTE LINIA CRITICĂ: Spune Azure-ului să ștergă mesajul
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                // AICI vei vedea de ce se repetă mesajele
                Console.WriteLine($"❌ EROARE CRITICĂ ÎN WORKER: {ex.Message}");
            }
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"❌ Eroare în PaymentWorker: {args.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}