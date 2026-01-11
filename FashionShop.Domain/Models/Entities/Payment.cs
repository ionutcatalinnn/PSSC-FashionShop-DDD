using System;
using FashionShop.Domain.Models.ValueObjects;

namespace FashionShop.Domain.Models.Entities
{
    public static class Payment
    {
        public interface IPayment { }

        public record UnvalidatedPayment(string OrderId, decimal Amount, string CardNumber, string Cvv) : IPayment;

        public record InvalidPayment(string Reason) : IPayment;

        public record ValidatedPayment(Guid OrderId, Price Amount, string CardNumber, string Cvv) : IPayment;
        
        public record VerifiedPayment(Guid OrderId, Price Amount, string CardNumber, string Cvv) : IPayment;

        public record ProcessedPayment(Guid PaymentId, Guid OrderId, Price Amount, DateTime ProcessedAt) : IPayment;
    }
}