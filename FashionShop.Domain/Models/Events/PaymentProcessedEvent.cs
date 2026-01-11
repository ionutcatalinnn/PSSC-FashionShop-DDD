using System;
using static FashionShop.Domain.Models.Entities.Payment;

namespace FashionShop.Domain.Models.Events
{
    public static class PaymentProcessedEvent
    {
        public interface IPaymentProcessedEvent { }

        public record PaymentSucceeded(Guid PaymentId, Guid OrderId, DateTime ProcessedAt) : IPaymentProcessedEvent;

        public record PaymentFailed(string Reason) : IPaymentProcessedEvent;

        public static IPaymentProcessedEvent ToEvent(this IPayment payment) =>
            payment switch
            {
                ProcessedPayment p => new PaymentSucceeded(p.PaymentId, p.OrderId, p.ProcessedAt),
                InvalidPayment i => new PaymentFailed(i.Reason),
                _ => new PaymentFailed("Unexpected payment state")
            };
    }
}