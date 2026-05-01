using Order.API.Domain.Enums;

namespace Order.API.Domain.Events;

public class OrderCreatedEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid BuyerId { get; init; }
    public decimal TotalPrice { get; init; }

    public OrderCreatedEvent(Guid orderId, Guid buyerId, decimal totalPrice)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        TotalPrice = totalPrice;
    }
}