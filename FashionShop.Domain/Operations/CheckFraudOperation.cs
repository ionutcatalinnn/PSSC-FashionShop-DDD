using static FashionShop.Domain.Models.Entities.Payment;

namespace FashionShop.Domain.Operations
{
    internal sealed class CheckFraudOperation : PaymentOperation
    {
        protected override IPayment OnValidated(ValidatedPayment payment)
        {
            // Simulare verificare antifraudă:
            // Dacă suma e mai mare de 10,000, o considerăm suspectă
            if (payment.Amount.Value > 10000)
            {
                return new InvalidPayment("Fraud check failed: Amount too high.");
            }

            // Dacă e ok, trecem în starea Verified
            return new VerifiedPayment(payment.OrderId, payment.Amount, payment.CardNumber, payment.Cvv);
        }
    }
}