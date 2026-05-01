namespace Stock.API.Domain.ValueObjects;

public readonly record struct Quantity(int Value)
{
    public static Quantity From(int value)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Quantity must be greater than 0");

        return new Quantity(value);
    }
}
