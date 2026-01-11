using System;
using FashionShop.Domain.Exceptions;
using FashionShop.Domain.Models.ValueObjects;
using static FashionShop.Domain.Models.Entities.Shipping;

namespace FashionShop.Domain.Operations
{
    // --- BASE OPERATION ---
    internal abstract class ShippingOperation : DomainOperation<IShipping, object, IShipping>
    {
        public override IShipping Transform(IShipping shipping, object? state = null) =>
            shipping switch
            {
                UnvalidatedShipping u => OnUnvalidated(u),
                ValidatedShipping v => OnValidated(v),
                PackagedOrder p => OnPackaged(p), // <--- NOU
                ShippedOrder s => OnShipped(s),
                InvalidShipping i => OnInvalid(i),
                _ => throw new InvalidOrderStateException(shipping.GetType().Name)
            };

        protected virtual IShipping OnUnvalidated(UnvalidatedShipping s) => s;
        protected virtual IShipping OnValidated(ValidatedShipping s) => s;
        protected virtual IShipping OnPackaged(PackagedOrder s) => s; // <--- NOU
        protected virtual IShipping OnShipped(ShippedOrder s) => s;
        protected virtual IShipping OnInvalid(InvalidShipping s) => s;
    }

    // --- 1. VALIDATE ---
    internal sealed class ValidateShippingOperation : ShippingOperation
    {
        protected override IShipping OnUnvalidated(UnvalidatedShipping shipping)
        {
            if (!Guid.TryParse(shipping.OrderId, out var orderId))
                return new InvalidShipping("Invalid Order ID.");

            try
            {
                var address = new ShippingAddress(shipping.City, shipping.Street);
                return new ValidatedShipping(orderId, address);
            }
            catch (Exception ex)
            {
                return new InvalidShipping(ex.Message);
            }
        }
    }

    // --- 2. PACKAGE (NOU) ---
    internal sealed class PackageOrderOperation : ShippingOperation
    {
        protected override IShipping OnValidated(ValidatedShipping shipping)
        {
            // Calculăm greutatea coletului (simulare)
            decimal weight = 2.5m; 
            return new PackagedOrder(shipping.OrderId, shipping.Address, weight);
        }
    }

    // --- 3. SHIP ---
    internal sealed class ShipOrderOperation : ShippingOperation
    {
        // Modificare: Primește PackagedOrder acum
        protected override IShipping OnPackaged(PackagedOrder shipping)
        {
            string awb = $"AWB-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            
            return new ShippedOrder(shipping.OrderId, shipping.Address, awb, DateTime.UtcNow);
        }
    }
}