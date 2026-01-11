using System;
using static FashionShop.Domain.Models.Entities.Payment;

namespace FashionShop.Domain.Operations
{
    internal sealed class ProcessPaymentOperation : PaymentOperation
    {
        // Modificare: Input-ul este acum VerifiedPayment
        protected override IPayment OnVerified(VerifiedPayment payment)
        {
            return new ProcessedPayment(
                PaymentId: Guid.NewGuid(),
                OrderId: payment.OrderId,
                Amount: payment.Amount,
                ProcessedAt: DateTime.UtcNow
            );
        }
    }
}