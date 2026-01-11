using System;

namespace FashionShop.Domain.Exceptions
{
    public class InvalidOrderStateException : Exception
    {
        public InvalidOrderStateException(string state)
            : base($"Order state {state} not implemented")
        {
        }
    }
}