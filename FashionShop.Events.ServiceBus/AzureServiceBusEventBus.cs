using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using FashionShop.Domain; // Folosim interfața din Domain

namespace FashionShop.Events.ServiceBus
{
    public class AzureServiceBusEventBus : IAsyncEventBus, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;
        private readonly ServiceBusReceiver _receiver;
        
        // Numele cozii trebuie să fie exact cel creat în Azure Portal
        private const string QueueName = "orders-queue";

        public AzureServiceBusEventBus(IConfiguration configuration)
        {
            // Citim cheia de conectare
            var connectionString = configuration.GetConnectionString("AzureServiceBus");
            
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender(QueueName);
            _receiver = _client.CreateReceiver(QueueName);
        }

        public async ValueTask PublishAsync<T>(T @event)
        {
            // Serializăm obiectul în JSON
            string jsonBody = JsonSerializer.Serialize(@event);
            
            var message = new ServiceBusMessage(jsonBody)
            {
                Subject = typeof(T).Name // Etichetăm mesajul cu tipul lui
            };

            await _sender.SendMessageAsync(message);
        }

        public async IAsyncEnumerable<T> SubscribeAsync<T>(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Așteptăm mesaje din Azure
                var message = await _receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);

                if (message != null)
                {
                    // Verificăm dacă mesajul este pentru noi
                    if (message.Subject == typeof(T).Name)
                    {
                        var body = message.Body.ToString();
                        var eventData = JsonSerializer.Deserialize<T>(body);

                        if (eventData != null)
                        {
                            yield return eventData;
                        }

                        // Ștergem mesajul din coadă (ACK)
                        await _receiver.CompleteMessageAsync(message, cancellationToken);
                    }
                    else
                    {
                        // Nu e pentru noi, îl lăsăm acolo
                        await _receiver.AbandonMessageAsync(message, null, cancellationToken);
                    }
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _receiver.DisposeAsync();
            await _sender.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}