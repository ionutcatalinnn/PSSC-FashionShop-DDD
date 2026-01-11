using FashionShop.Domain.Exceptions;
using static FashionShop.Domain.Models.Entities.Order;

namespace FashionShop.Domain.Operations
{
  internal abstract class OrderOperation<TState>
    : DomainOperation<IOrder, TState, IOrder>
    where TState : class
  {
    public override IOrder Transform(IOrder order, TState? state) =>
      order switch
      {
        UnvalidatedOrder unvalidated => OnUnvalidated(unvalidated, state),
        ValidatedOrder validated => OnValidated(validated, state),
        CalculatedOrder calculated => OnCalculated(calculated, state),
        PlacedOrder placed => OnPlaced(placed, state),
        InvalidOrder invalid => OnInvalid(invalid, state),
        _ => throw new InvalidOrderStateException(order.GetType().Name)
      };

    protected virtual IOrder OnUnvalidated(UnvalidatedOrder order, TState? state) => order;
    protected virtual IOrder OnValidated(ValidatedOrder order, TState? state) => order;
    protected virtual IOrder OnCalculated(CalculatedOrder order, TState? state) => order;
    protected virtual IOrder OnPlaced(PlacedOrder order, TState? state) => order;
    protected virtual IOrder OnInvalid(InvalidOrder order, TState? state) => order;
  }

  internal abstract class OrderOperation : OrderOperation<object>
  {
    internal IOrder Transform(IOrder order) => Transform(order, null);

    protected sealed override IOrder OnUnvalidated(UnvalidatedOrder order, object? _) =>
      OnUnvalidated(order);

    protected virtual IOrder OnUnvalidated(UnvalidatedOrder order) => order;

    protected sealed override IOrder OnValidated(ValidatedOrder order, object? _) =>
      OnValidated(order);

    protected virtual IOrder OnValidated(ValidatedOrder order) => order;

    protected sealed override IOrder OnCalculated(CalculatedOrder order, object? _) =>
      OnCalculated(order);

    protected virtual IOrder OnCalculated(CalculatedOrder order) => order;

    protected sealed override IOrder OnPlaced(PlacedOrder order, object? _) =>
      OnPlaced(order);

    protected virtual IOrder OnPlaced(PlacedOrder order) => order;

    protected sealed override IOrder OnInvalid(InvalidOrder order, object? _) =>
      OnInvalid(order);

    protected virtual IOrder OnInvalid(InvalidOrder order) => order;
  }
}
