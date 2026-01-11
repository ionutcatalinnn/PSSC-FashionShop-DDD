using System.Collections.Generic;
using FashionShop.Domain.Models.Entities;

namespace FashionShop.Domain.Models.Commands
{
    public record PlaceOrderCommand(
        IReadOnlyCollection<UnvalidatedOrderLine> Lines);
}