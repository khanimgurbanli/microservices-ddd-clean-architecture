namespace Stock.API.Domain.Events;

public class StockNotReservedEvent : DomainEvent
{
    public Guid StockId { get; }
    public Guid ProductId { get; }
    public string Reason { get; }

    public StockNotReservedEvent(Guid stockId, Guid productId, string reason)
    {
        StockId = stockId;
        ProductId = productId;
        Reason = reason;
    }
}
