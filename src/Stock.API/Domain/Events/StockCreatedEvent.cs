namespace Stock.API.Domain.Events;

public class StockCreatedEvent : DomainEvent
{
    public Guid StockId { get; }
    public Guid ProductId { get; }
    public int InitialCount { get; }

    public StockCreatedEvent(Guid stockId, Guid productId, int initialCount)
    {
        StockId = stockId;
        ProductId = productId;
        InitialCount = initialCount;
    }
}
