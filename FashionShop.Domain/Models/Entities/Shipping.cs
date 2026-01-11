using System;
using FashionShop.Domain.Models.ValueObjects;

namespace FashionShop.Domain.Models.Entities
{
    public static class Shipping
    {
        public interface IShipping { }

        public record UnvalidatedShipping(string OrderId, string City, string Street) : IShipping;

        public record InvalidShipping(string Reason) : IShipping;

        public record ValidatedShipping(Guid OrderId, ShippingAddress Address) : IShipping;
        
        public record PackagedOrder(Guid OrderId, ShippingAddress Address, decimal Weight) : IShipping;

        public record ShippedOrder(Guid OrderId, ShippingAddress Address, string AWB, DateTime ShippedAt) : IShipping;
    }
}