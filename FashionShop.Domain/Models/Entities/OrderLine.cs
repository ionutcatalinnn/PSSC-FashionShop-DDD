namespace FashionShop.Domain.Models.Entities
{
    // 1. Linia brută 
    public record UnvalidatedOrderLine(string ProductCode, int Quantity);

    // 2. Linia validată (știm că produsul există și cantitatea e pozitivă)
    public record ValidatedOrderLine(string ProductCode, int Quantity);

    // 3. Linia calculată (avem prețul unitar și prețul total pe linie)
    public record CalculatedOrderLine(string ProductCode, int Quantity, decimal UnitPrice, decimal LineTotal);
}