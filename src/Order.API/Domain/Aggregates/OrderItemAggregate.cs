namespace Order.API.Domain.Aggregates;

public class OrderItemAggregate
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Count { get; private set; }
    public decimal Price { get; private set; }
    public Guid OrderId { get; private set; }

    private OrderItemAggregate() { }

    public static OrderItemAggregate Create(Guid orderId, Guid productId, int count, decimal price)
    {
        return new OrderItemAggregate
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = productId,
            Count = count,
            Price = price
        };
    }

    public static OrderItemAggregate Load(Guid id, Guid orderId, Guid productId, int count, decimal price)
    {
        return new OrderItemAggregate
        {
            Id = id,
            OrderId = orderId,
            ProductId = productId,
            Count = count,
            Price = price
        };
    }
}