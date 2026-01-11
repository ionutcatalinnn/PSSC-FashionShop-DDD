using System;
using FashionShop.Domain.Exceptions;

namespace FashionShop.Domain.Models.ValueObjects
{
    public record ShippingAddress
    {
        public string City { get; }
        public string Street { get; }

        public ShippingAddress(string city, string street)
        {
            if (string.IsNullOrWhiteSpace(city)) throw new Exception("City is required");
            if (string.IsNullOrWhiteSpace(street)) throw new Exception("Street is required");

            City = city;
            Street = street;
        }

        public override string ToString() => $"{City}, {Street}";
    }
}