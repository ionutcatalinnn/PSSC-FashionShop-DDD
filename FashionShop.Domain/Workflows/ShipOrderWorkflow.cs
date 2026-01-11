using FashionShop.Domain.Models.Commands;
using FashionShop.Domain.Models.Events;
using FashionShop.Domain.Operations;
using static FashionShop.Domain.Models.Entities.Shipping;
using static FashionShop.Domain.Models.Events.OrderShippedEvent;

namespace FashionShop.Domain.Workflows
{
    public class ShipOrderWorkflow
    {
        public IOrderShippedEvent Execute(ShipOrderCommand command)
        {
            IShipping shipping = new UnvalidatedShipping(command.OrderId, command.City, command.Street);

            // 1. Validate
            shipping = new ValidateShippingOperation().Transform(shipping);
            
            // 2. Package (NOU)
            shipping = new PackageOrderOperation().Transform(shipping);
            
            // 3. Ship
            shipping = new ShipOrderOperation().Transform(shipping);

            return shipping.ToEvent();
        }
    }
}