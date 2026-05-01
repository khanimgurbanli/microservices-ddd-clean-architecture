namespace Order.API.Domain.ValueObjects;

public readonly record struct ProductId(Guid Value)
{
    public static ProductId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(value));

        return new ProductId(value);
    }
}
