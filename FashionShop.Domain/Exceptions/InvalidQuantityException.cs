using System;

namespace FashionShop.Domain.Exceptions
{
    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(int value)
            : base($"Invalid quantity: {value}")
        {
        }
    }
}