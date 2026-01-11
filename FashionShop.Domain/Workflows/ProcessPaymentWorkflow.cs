using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Models.Events;
using FashionShop.Domain.Operations;
using static FashionShop.Domain.Models.Entities.Payment;
using static FashionShop.Domain.Models.Events.PaymentProcessedEvent;

namespace FashionShop.Domain.Workflows
{
    public class ProcessPaymentWorkflow
    {
        public IPaymentProcessedEvent Execute(PayOrderCommand command)
        {
            IPayment payment = new UnvalidatedPayment(command.OrderId, command.Amount, command.CardNumber, command.Cvv);

            // 1. Validate
            payment = new ValidatePaymentOperation().Transform(payment);
            
            // 2. Check Fraud (NOU)
            payment = new CheckFraudOperation().Transform(payment);
            
            // 3. Process
            payment = new ProcessPaymentOperation().Transform(payment);

            return payment.ToEvent();
        }
    }
}