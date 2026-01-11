using FashionShop.Domain.Models.Entities;
using static FashionShop.Domain.Models.Entities.Order;

namespace FashionShop.Domain.Repositories
{
    public interface IOrderRepository
    {
        // Metoda care primește comanda finalizată și o salvează
        void Save(PlacedOrder order);
    }
}