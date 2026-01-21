using System;

namespace FashionShop.Domain.Models.Events
{
    // Acest eveniment va fi trimis către Shipping după ce plata e confirmată
    public record OrderPaidEvent(Guid OrderId, decimal Amount, DateTime PaidAt);
}