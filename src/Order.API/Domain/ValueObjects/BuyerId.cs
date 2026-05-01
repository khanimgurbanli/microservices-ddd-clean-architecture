namespace Order.API.Domain.ValueObjects;

public readonly record struct BuyerId(Guid Value)
{
    public static BuyerId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("BuyerId cannot be empty", nameof(value));

        return new BuyerId(value);
    }
}
