namespace FashionShop.Domain.Models.Commands
{
    public record ShipOrderCommand(string OrderId, string City, string Street);
}