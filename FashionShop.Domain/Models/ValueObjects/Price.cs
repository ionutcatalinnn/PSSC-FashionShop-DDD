using System;
using FashionShop.Domain.Exceptions;

namespace FashionShop.Domain.Models.ValueObjects
{
    public record Price
    {
        public decimal Value { get; }

        public Price(decimal value)
        {
            if (value < 0)
                throw new InvalidPriceException("Price cannot be negative.");

            Value = value;
        }

        // Operator implicit pentru a putea folosi Price ca decimal ușor (opțional, dar util)
        public static implicit operator decimal(Price price) => price.Value;
        
        public override string ToString() => $"{Value:0.00}";
    }
}