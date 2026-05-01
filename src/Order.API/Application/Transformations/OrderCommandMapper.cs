using Order.API.Application.Commands.Order;
using Order.API.Domain.Aggregates;

namespace Order.API.Application.Transformations;

public static class OrderCommandMapper
{
    public static OrderAggregate ToAggregate(CreateOrderCommand command)
    {
        var order = OrderAggregate.Create(command.BuyerId);

        foreach (var item in command.OrderItems)
        {
            order.AddItem(item.ProductId, item.Count, item.Price);
        }

        return order;
    }
}
