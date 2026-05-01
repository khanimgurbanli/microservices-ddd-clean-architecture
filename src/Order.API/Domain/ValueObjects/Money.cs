namespace Order.API.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        Amount = amount;
        Currency = currency;
    }

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Currency mismatch");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money Zero => new(0);
}