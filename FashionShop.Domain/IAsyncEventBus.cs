using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FashionShop.Domain
{
    public interface IAsyncEventBus
    {
        // Trimite un mesaj
        ValueTask PublishAsync<T>(T @event);

        // Ascultă mesaje
        IAsyncEnumerable<T> SubscribeAsync<T>(CancellationToken cancellationToken);
    }
}