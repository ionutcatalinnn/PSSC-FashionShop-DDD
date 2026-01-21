using System.Collections.Generic;

namespace FashionShop.Domain.Models.Commands
{
    // 1. Comanda principală
    public record PlaceOrderCommand(
        List<OrderLineInput> Lines, 
        string CustomerName, 
        string Address
    );

    // 2. AICI DEFINIM CE ÎNSEAMNĂ "OrderLineInput" (Linia care lipsea)
    public record OrderLineInput(
        string ProductCode, 
        int Quantity,
        decimal Price
    );
}