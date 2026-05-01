namespace Stock.API.Domain.Events;

public class StockReservedDomainEvent : DomainEvent
{
    public Guid StockId { get; }
    public Guid ProductId { get; }
    public int Quantity { get; }

    public StockReservedDomainEvent(Guid stockId, Guid productId, int quantity)
    {
        StockId = stockId;
        ProductId = productId;
        Quantity = quantity;
    }
}
