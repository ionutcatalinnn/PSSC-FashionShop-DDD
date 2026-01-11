using FashionShop.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace FashionShop.Domain.Models.ValueObjects
{
    public record ProductCode
    {
        private static readonly Regex ValidPattern = new("^[A-Z]{3}-[0-9]{3}$");
        public string Value { get; }

        private ProductCode(string value)
        {
            if (!ValidPattern.IsMatch(value))
                throw new InvalidProductCodeException(value);

            Value = value;
        }

        public static bool TryParse(string value, out ProductCode? code)
        {
            code = null;
            if (!ValidPattern.IsMatch(value)) return false;

            code = new ProductCode(value);
            return true;
        }

        public override string ToString() => Value;
    }
}