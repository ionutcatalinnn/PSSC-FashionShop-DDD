using FashionShop.Domain.Exceptions;

namespace FashionShop.Domain.Models.ValueObjects
{
    public record Quantity
    {
        public int Value { get; }

        public Quantity(int value)
        {
            if (value <= 0)
                throw new InvalidQuantityException(value);

            Value = value;
        }
    }
}