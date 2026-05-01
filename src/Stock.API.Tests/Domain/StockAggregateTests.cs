using Xunit;
using Stock.API.Domain.Aggregates;

namespace Stock.API.Tests.Domain;

public class StockAggregateTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateStockWithCorrectValues()
    {
        var productId = Guid.NewGuid();
        var count = 100;

        var stock = StockAggregate.Create(productId, count);

        Assert.NotEqual(Guid.Empty, stock.Id);
        Assert.Equal(productId, stock.ProductId);
        Assert.Equal(count, stock.Count);
    }

    [Fact]
    public void ReserveStock_SufficientQuantity_ShouldDecreaseCount()
    {
        var productId = Guid.NewGuid();
        var stock = StockAggregate.Create(productId, 100);

        stock.ReserveStock(30);

        Assert.Equal(70, stock.Count);
    }

    [Fact]
    public void ReserveStock_InsufficientQuantity_ShouldNotDecreaseCount()
    {
        var productId = Guid.NewGuid();
        var stock = StockAggregate.Create(productId, 10);

        stock.ReserveStock(50);

        Assert.Equal(10, stock.Count);
    }

    [Fact]
    public void Load_ShouldRestoreStockCorrectly()
    {
        var id = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var count = 75;

        var stock = StockAggregate.Load(id, productId, count);

        Assert.Equal(id, stock.Id);
        Assert.Equal(productId, stock.ProductId);
        Assert.Equal(count, stock.Count);
    }
}
