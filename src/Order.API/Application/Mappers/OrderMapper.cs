using Order.API.Application.Models.Orders;
using Order.API.Domain.Aggregates;

namespace Order.API.Application.Mappers;

public static class OrderMapper
{
    public static OrderResponse MapToResponse(OrderAggregate order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            BuyerId = order.BuyerId,
            TotalPrice = order.TotalPrice,
            OrderStatus = (int)order.OrderStatus,
            CreatedDate = order.CreatedDate,
            OrderItems = order.Items.Select(oi => new OrderItemResponse
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                Count = oi.Count,
                Price = oi.Price
            }).ToList()
        };
    }

    public static IEnumerable<OrderResponse> MapToResponse(IEnumerable<OrderAggregate> orders)
    {
        return orders.Select(MapToResponse);
    }
}
