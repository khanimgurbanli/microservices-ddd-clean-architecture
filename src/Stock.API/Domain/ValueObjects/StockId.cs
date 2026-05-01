namespace Stock.API.Domain.ValueObjects;

public readonly record struct StockId(Guid Value)
{
    public static StockId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("StockId cannot be empty", nameof(value));

        return new StockId(value);
    }
}
