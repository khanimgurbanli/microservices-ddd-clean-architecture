using Order.API.Domain.Enums;

namespace Order.API.Application.Events;

public class OrderStatusChangedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public OrderStatus NewStatus { get; init; }
    public DateTime ChangedAt { get; init; }
}