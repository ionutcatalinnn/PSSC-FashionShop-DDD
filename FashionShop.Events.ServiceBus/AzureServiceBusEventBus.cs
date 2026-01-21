using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using FashionShop.Domain;
using FashionShop.Domain.Models.Events;

namespace FashionShop.Events.ServiceBus
{
    public class AzureServiceBusEventBus : IAsyncEventBus, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly IConfiguration _configuration;

        public AzureServiceBusEventBus(IConfiguration configuration)
        {
            _configuration = configuration;
            var connectionString = _configuration.GetConnectionString("AzureServiceBus");
            _client = new ServiceBusClient(connectionString);
        }

        public async ValueTask PublishAsync<T>(T @event)
        {
            // Rutare inteligentă: Plățile merg la 'payments', restul la 'orders'
            string targetQueue = @event switch
            {
                OrderPaidEvent => "payments",
                _ => "orders"
            };

            await using var sender = _client.CreateSender(targetQueue);
            string jsonBody = JsonSerializer.Serialize(@event);
            
            var message = new ServiceBusMessage(jsonBody) { Subject = typeof(T).Name };
            await sender.SendMessageAsync(message);
        }

        public async IAsyncEnumerable<T> SubscribeAsync<T>(CancellationToken cancellationToken)
        {
            // IMPORTANT: ShippingWorker va cere T = OrderPaidEvent, deci va asculta pe 'payments'
            string listenQueue = typeof(T).Name == nameof(OrderPaidEvent) ? "payments" : "orders";
            
            await using var receiver = _client.CreateReceiver(listenQueue);

            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);
                if (message != null)
                {
                    if (message.Subject == typeof(T).Name)
                    {
                        var eventData = JsonSerializer.Deserialize<T>(message.Body.ToString());
                        if (eventData != null) yield return eventData;
                        await receiver.CompleteMessageAsync(message, cancellationToken);
                    }
                    else
                    {
                        await receiver.AbandonMessageAsync(message, null, cancellationToken);
                    }
                }
            }
        }

        public async ValueTask DisposeAsync() => await _client.DisposeAsync();
    }
}