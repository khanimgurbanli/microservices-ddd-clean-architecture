using Order.API.Domain.ValueObjects;

namespace Order.API.Tests.Domain;

public class ValueObjectsTests
{
    [Fact]
    public void Money_Constructor_WithValidAmount_ShouldCreate()
    {
        var money = new Money(100.50m, "USD");

        Assert.Equal(100.50m, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Money_Constructor_WithNegativeAmount_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new Money(-10m));
    }

    [Fact]
    public void Money_Addition_ShouldAddAmounts()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "USD");

        var result = money1 + money2;

        Assert.Equal(150m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Money_Zero_ShouldReturnZeroMoney()
    {
        var zero = Money.Zero;

        Assert.Equal(0m, zero.Amount);
        Assert.Equal("USD", zero.Currency);
    }
}