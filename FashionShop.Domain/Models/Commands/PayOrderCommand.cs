namespace FashionShop.Domain.Models.Commands
{
    public record PayOrderCommand(string OrderId, decimal Amount, string CardNumber, string Cvv);
}