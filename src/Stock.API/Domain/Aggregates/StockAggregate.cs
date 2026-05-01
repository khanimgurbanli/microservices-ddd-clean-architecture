using Stock.API.Domain.Events;

namespace Stock.API.Domain.Aggregates;

public class StockAggregate
{
    private readonly List<DomainEvent> _domainEvents = new();

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Count { get; private set; }
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private StockAggregate() { }

    public static StockAggregate Create(Guid productId, int count)
    {
        var stock = new StockAggregate
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Count = count
        };

        stock._domainEvents.Add(new StockCreatedEvent(stock.Id, stock.ProductId, stock.Count));
        return stock;
    }

    public bool ReserveStock(int quantity)
    {
        if (Count < quantity)
        {
            _domainEvents.Add(new StockNotReservedEvent(Id, ProductId, "Insufficient stock"));
            return false;
        }

        Count -= quantity;
        _domainEvents.Add(new StockReservedDomainEvent(Id, ProductId, quantity));
        return true;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public static StockAggregate Load(Guid id, Guid productId, int count)
    {
        return new StockAggregate
        {
            Id = id,
            ProductId = productId,
            Count = count
        };
    }
}
