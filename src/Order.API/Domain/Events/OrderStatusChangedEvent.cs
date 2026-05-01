using Order.API.Domain.Enums;

namespace Order.API.Domain.Events;

public class OrderStatusChangedEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public OrderStatus OldStatus { get; init; }
    public OrderStatus NewStatus { get; init; }

    public OrderStatusChangedEvent(Guid orderId, OrderStatus oldStatus, OrderStatus newStatus)
    {
        OrderId = orderId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}