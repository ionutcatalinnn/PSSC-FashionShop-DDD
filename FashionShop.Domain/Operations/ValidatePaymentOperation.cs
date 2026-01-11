using System;
using FashionShop.Domain.Models.ValueObjects;
using static FashionShop.Domain.Models.Entities.Payment;

namespace FashionShop.Domain.Operations
{
    internal sealed class ValidatePaymentOperation : PaymentOperation
    {
        protected override IPayment OnUnvalidated(UnvalidatedPayment payment)
        {
            if (!Guid.TryParse(payment.OrderId, out var orderId))
                return new InvalidPayment("Invalid Order ID format.");

            if (payment.Amount <= 0)
                return new InvalidPayment("Amount must be greater than 0.");

            if (string.IsNullOrWhiteSpace(payment.CardNumber) || payment.CardNumber.Length < 16)
                return new InvalidPayment("Invalid Card Number.");

            // Aici transformăm în Value Objects
            try 
            {
                var price = new Price(payment.Amount);
                return new ValidatedPayment(orderId, price, payment.CardNumber, payment.Cvv);
            }
            catch(Exception ex)
            {
                return new InvalidPayment(ex.Message);
            }
        }
    }
}