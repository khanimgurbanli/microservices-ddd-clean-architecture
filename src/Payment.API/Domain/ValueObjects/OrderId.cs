namespace Payment.API.Domain.ValueObjects;

public readonly record struct OrderId(Guid Value)
{
    public static OrderId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty", nameof(value));

        return new OrderId(value);
    }
}
