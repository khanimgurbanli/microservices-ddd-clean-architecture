using Payment.API.Domain.Aggregates;
using Payment.API.Domain.ValueObjects;
using Shared.Events;

namespace Payment.API.Infrastructure.Transformations;

public static class StockReservedEventMapper
{
    public static PaymentAggregate ToAggregate(StockReservedEvent @event)
    {
        return PaymentAggregate.Create(OrderId.From(@event.OrderId));
    }
}
