using Shared.Events;
using Shared.Messages;
using Stock.API.Domain.ValueObjects;

namespace Stock.API.Infrastructure.Transformations;

public static class CreateOrderEventMapper
{
    public static IEnumerable<StockReservationItem> MapToReservationItems(CreateOrderEvent @event)
    {
        return @event.OrderItems.Select(static (OrderItemMessage item) =>
            new StockReservationItem(ProductId.From(item.ProductId), Quantity.From(item.Count)));
    }
}
