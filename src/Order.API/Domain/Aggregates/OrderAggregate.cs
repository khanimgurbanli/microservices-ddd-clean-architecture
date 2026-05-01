using Order.API.Domain.Enums;
using Order.API.Domain.Events;

namespace Order.API.Domain.Aggregates;

public class OrderAggregate
{
    private readonly List<DomainEvent> _domainEvents = new();
    private readonly List<OrderItemAggregate> _items = new();

    public Guid Id { get; private set; }
    public Guid BuyerId { get; private set; }
    public decimal TotalPrice { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public IReadOnlyCollection<OrderItemAggregate> Items => _items.AsReadOnly();

    private OrderAggregate() { }

    public static OrderAggregate Create(Guid buyerId)
    {
        var order = new OrderAggregate
        {
            Id = Guid.NewGuid(),
            BuyerId = buyerId,
            OrderStatus = OrderStatus.Suspended,
            CreatedDate = DateTime.UtcNow
        };

        order._domainEvents.Add(new OrderCreatedEvent(order.Id, order.BuyerId, 0));
        return order;
    }

    public void AddItem(Guid productId, int count, decimal price)
    {
        if (OrderStatus != OrderStatus.Suspended)
            throw new InvalidOperationException("Cannot add items to a non-suspended order");

        var item = OrderItemAggregate.Create(Id, productId, count, price);
        _items.Add(item);
        RecalculateTotalPrice();
    }

    public void RecalculateTotalPrice()
    {
        TotalPrice = _items.Sum(i => i.Price * i.Count);
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        var oldStatus = OrderStatus;
        OrderStatus = newStatus;

        if (oldStatus != newStatus)
        {
            _domainEvents.Add(new OrderStatusChangedEvent(Id, oldStatus, newStatus));
        }
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public static OrderAggregate Load(Guid id, Guid buyerId, OrderStatus status, decimal totalPrice, DateTime createdDate, List<OrderItemAggregate> items)
    {
        var order = new OrderAggregate
        {
            Id = id,
            BuyerId = buyerId,
            OrderStatus = status,
            TotalPrice = totalPrice,
            CreatedDate = createdDate
        };
        order._items.AddRange(items);
        return order;
    }
}