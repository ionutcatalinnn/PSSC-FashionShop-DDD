using FashionShop_Hub.Data.Models;
using static FashionShop.Domain.Models.Entities.Payment;

namespace FashionShop_Hub.Data.Repositories
{
    public class PaymentRepository
    {
        private readonly FashionDbContext _ctx;
        public PaymentRepository(FashionDbContext ctx) => _ctx = ctx;

        public void Save(ProcessedPayment payment)
        {
            _ctx.Payments.Add(new PaymentDto
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                Amount = payment.Amount.Value,
                ProcessedAt = payment.ProcessedAt,
                Status = "Processed"
            });
            _ctx.SaveChanges();
        }
    }
}