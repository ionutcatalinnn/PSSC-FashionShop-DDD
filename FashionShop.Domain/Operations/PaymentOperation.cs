using FashionShop.Domain.Exceptions;
using static FashionShop.Domain.Models.Entities.Payment;

namespace FashionShop.Domain.Operations
{
    internal abstract class PaymentOperation : DomainOperation<IPayment, object, IPayment>
    {
        public override IPayment Transform(IPayment payment, object? state = null) =>
            payment switch
            {
                UnvalidatedPayment u => OnUnvalidated(u),
                ValidatedPayment v => OnValidated(v),
                VerifiedPayment ver => OnVerified(ver), // <--- NOU
                ProcessedPayment p => OnProcessed(p),
                InvalidPayment i => OnInvalid(i),
                _ => throw new InvalidOrderStateException(payment.GetType().Name)
            };

        protected virtual IPayment OnUnvalidated(UnvalidatedPayment payment) => payment;
        protected virtual IPayment OnValidated(ValidatedPayment payment) => payment;
        protected virtual IPayment OnVerified(VerifiedPayment payment) => payment; // <--- NOU
        protected virtual IPayment OnProcessed(ProcessedPayment payment) => payment;
        protected virtual IPayment OnInvalid(InvalidPayment payment) => payment;
    }
}