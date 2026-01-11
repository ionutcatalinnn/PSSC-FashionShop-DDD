using System;

namespace FashionShop.Domain.Exceptions
{
    public class InvalidProductCodeException : Exception
    {
        public InvalidProductCodeException(string value)
            : base($"Invalid product code: {value}")
        {
        }
    }
}