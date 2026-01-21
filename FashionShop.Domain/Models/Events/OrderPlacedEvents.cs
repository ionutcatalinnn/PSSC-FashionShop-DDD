using System;
using FashionShop.Domain.Models; // Asigură-te că ai namespace-ul pentru IEvent (dacă există)

namespace FashionShop.Domain.Models.Events
{
    // AICI ERA EROAREA: Înainte era "public static class".
    // ACUM este un simplu "record" pe care îl poți crea cu 'new'.
    
    public record OrderPlacedEvent(Guid OrderId, decimal Total, DateTime PlacedAt) ;
}